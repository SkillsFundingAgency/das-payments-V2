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
                .Where(x => x.LearnerUln == LearnerUln).ToList()
                ;

            var lockedCommitments = context.DataLockFailure
                .Where(x => x.LearnerUln == LearnerUln).ToList()
                ;

            var apprenticeships = context.Apprenticeship.Where(x => x.Uln == LearnerUln).Include(x => x.ApprenticeshipPriceEpisodes).ToList();
            var ape = apprenticeships.SelectMany(x => x.ApprenticeshipPriceEpisodes).ToList();

            CollectionPeriods = earnings.Select(periods => new CollectionPeriod
            {
                PeriodName = $"R0{periods.Key}",
                PriceEpisodes = periods.SelectMany(x => x.PriceEpisodes, (period, earning) => MapPriceEpisode(period, earning, paidCommitments, lockedCommitments, ape)).ToList(),
            }).ToList();
        }

        private static PriceEpisode MapPriceEpisode(
            EarningEventModel earning,
            EarningEventPriceEpisodeModel episode,
            List<PaymentModel> paidCommitments,
            List<DataLockFailureModel> lockedCommitments,
            List<ApprenticeshipPriceEpisodeModel> commitments)
        {
            return new PriceEpisode(
                episode.PriceEpisodeIdentifier,
                $"ACT{earning.ContractType}",
                episode.AgreedPrice,
                commitments.Select(x => new Commitment
                {
                    Id = x.Id,
                    Start = x.StartDate,
                    End = x.EndDate,
                    Provider = earning.Ukprn,
                    Payments = paidCommitments
                        .Where(y => (y.ApprenticeshipPriceEpisodeId?.Equals(x.Id)) ?? false)
                        .Select(y => new Payment
                        {
                            Amount = y.Amount,
                        }).ToList(),
                    DataLocked = lockedCommitments
                        .Where(y => y.CollectionPeriod == earning.CollectionPeriod)
                        .Select(y => new DataLock
                        {
                            Amount = y.Amount,
                        }).ToList(),
                }));
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

        public List<TransactionType> TransactionTypes { get; set; }

        public string Type => "Payment";
    }

    public class TransactionType
    {
        public int TypeId { get; set; }
    }
}