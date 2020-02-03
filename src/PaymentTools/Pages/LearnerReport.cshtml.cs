using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PaymentTools.Model;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentTools.Pages
{
    public class ConfigurablePaymentsDataContext : PaymentsDataContext
    {
        public ConfigurablePaymentsDataContext(DbContextOptions<ConfigurablePaymentsDataContext> options) 
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // do not call base
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder, enforceRequired: false);
        }
    }

    public class LearnerReportModel : PageModel
    {
        private readonly IPaymentsDataContext context;

        public LearnerReportModel(IPaymentsDataContext context)
        {
            this.context = context;
        }
        [BindProperty(SupportsGet = true)]
        public long LearnerUln { get; set; } = 942766;

        public IEnumerable<CollectionPeriod> CollectionPeriods { get; private set; }
        
        public void OnGet()
        {
            var earnings = context.EarningEvent
                .Where(x => x.LearnerUln == LearnerUln)
                .Include(x => x.PriceEpisodes)
                .AsEnumerable()
                .GroupBy(x => x.CollectionPeriod)
                .ToList();

            var paidCommitments = context.Payment
                .Where(x => x.LearnerUln == LearnerUln)
                //.Where(x => x.PriceEpisodeIdentifier)
                .ToList()
                ;

            //var lockedCommitments = context.DataLockFailure
            //    .Where(x => x.LearnerUln == LearnerUln).ToList()
            //    ;

            var apprenticeships = context.Apprenticeship.Where(x => x.Uln == LearnerUln).Include(x => x.ApprenticeshipPriceEpisodes).ToList();
            var ape = apprenticeships.SelectMany(x => x.ApprenticeshipPriceEpisodes).ToList();

            CollectionPeriods = earnings.Select(periods => new CollectionPeriod
            {
                PeriodName = $"R0{periods.Key}",
                PriceEpisodes = periods.SelectMany(x => x.PriceEpisodes, (period, earning) => MapPriceEpisode(period, earning, paidCommitments/*, lockedCommitments*/, apprenticeships)).ToList(),
            }).ToList();
        }

        private static PriceEpisode MapPriceEpisode(
            EarningEventModel earning,
            EarningEventPriceEpisodeModel episode,
            List<PaymentModel> paidCommitments,
            //List<DataLockFailureModel> lockedCommitments,
            List<ApprenticeshipModel> commitments)
        {
            return new PriceEpisode(
                episode.PriceEpisodeIdentifier,
                earning.ContractType.ToString(),
                episode.AgreedPrice,
                commitments.SelectMany(x => x.ApprenticeshipPriceEpisodes, (c, x) => new Commitment
                {
                    Id = x.Id,
                    Start = x.StartDate,
                    End = x.EndDate,
                    Employer = c.AccountId,
                    Provider = earning.Ukprn,
                    Course = FrameworkString(c),
                    Status = c.Status,
                    Cost = x.Cost,
                    Payments = paidCommitments
                        .Where(y => y.PriceEpisodeIdentifier == episode.PriceEpisodeIdentifier)
                        .Where(y => y.CollectionPeriod.Period <= earning.CollectionPeriod)
                        .Select(y => new Payment
                        {
                            Id = y.Id,
                            Amount = y.Amount,
                            PriceEpisodeIdentifier = y.PriceEpisodeIdentifier,
                            CollectionPeriod = y.CollectionPeriod,
                            TransactionType = y.TransactionType.ToString(),
                        }).ToList(),
                    //DataLocked = lockedCommitments
                    //    .Where(y => y.CollectionPeriod == earning.CollectionPeriod)
                    //    .Select(y => new DataLock
                    //    {
                    //        Amount = y.Amount,
                    //    }).ToList(),
                }));
        }

        private static string FrameworkString(ApprenticeshipModel c)
        {
            return $"{StandardString(c)}-{c.AgreedOnDate:dd-MM-yyyy}";
            
        }

        private static string StandardString(ApprenticeshipModel c)
        {
            return c.ProgrammeType == 0 ? $"25-{c.StandardCode}-" : $"{c.FrameworkCode}-{c.ProgrammeType}-{c.PathwayCode}";
        }
    }
}

namespace PaymentTools.Model
{
    public class CollectionPeriod
    {
        public string PeriodName { get; set; } = "R11";
        public decimal TotalPayments => PriceEpisodes.SelectMany(x => x.Commitments).Sum(x => x.TotalPayments);
        public decimal TotalLocked => PriceEpisodes.SelectMany(x => x.Commitments).SelectMany(x => x.Items).Select(x => x as DataLock).Where(x => x != null).Sum(x => x.Amount);
        public List<PriceEpisode> PriceEpisodes { get; set; }

    }
    public class PriceEpisode
    {
        public PriceEpisode(string name, string act, decimal price, IEnumerable<Commitment> commitments)
        {
            EpisodeName = name;
            Act = act;
            Price = price;
            Commitments = commitments.ToList().AsReadOnly();
        }

        public string EpisodeName { get; }
        public string Act { get; }
        public decimal Price { get; }
        public IReadOnlyList<Commitment> Commitments { get; } = new List<Commitment>();
    }

    public class Commitment
    {
        public long Id { get; set; }

        public string Description => $"{Start:MMM-yy} - {End:MMM-yy}";
        public long Provider { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset? End { get; set; }

        public List<CommitmentItem> Items => Payments.Cast<CommitmentItem>().Union(DataLocked).ToList();
        public List<DataLock> DataLocked { get; set; } = new List<DataLock>();
        public List<Payment> Payments { get; set; } = new List<Payment>();

        public decimal TotalPayments => Items.Where(x => x is Payment).Sum(x => (x as Payment).Amount);

        public long Employer { get; internal set; }
        public decimal Cost { get; internal set; }
        public ApprenticeshipStatus Status { get; internal set; }
        public string Course { get; internal set; }
    }

    public interface CommitmentItem
    {
        public string Type { get; }

        public decimal Amount { get; }
    }

    public class DataLock : CommitmentItem
    {
        public long Id { get; set; }

        public decimal Amount { get; set; }

        public int DataLockNumber { get; set; }

        public string Type => "Data Lock";
    }

    public class Payment : CommitmentItem
    {
        public long Id { get; set; }

        public decimal Amount { get; set; }

        public string TransactionType { get; set; }

        public string Type => "Payment";

        public string PriceEpisodeIdentifier { get; internal set; }
        public SFA.DAS.Payments.Model.Core.CollectionPeriod CollectionPeriod { get; internal set; }
    }

    public class TransactionType
    {
        public int TypeId { get; set; }
    }
}