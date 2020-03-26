using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PaymentTools.Model;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace PaymentTools.Pages
{
    public class LearnerReportModel : PageModel
    {
        private readonly IPaymentsDataContext context;

        public LearnerReportModel(IPaymentsDataContext context)
        {
            this.context = context;
        }
        [BindProperty(SupportsGet = true)]
        public long LearnerUln { get; set; } = 377103;

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

            var lockedCommitmentsQueryable = context.DataLockgEvent
                .Include(de => de.NonPayablePeriods)
                .ThenInclude(npp => npp.DataLockEventNonPayablePeriodFailures)
                .Where(x => x.LearnerUln == LearnerUln);//.ToList();

            var lockedCommitments = lockedCommitmentsQueryable.ToList();

            var apprenticeships = context.Apprenticeship.Where(x => x.Uln == LearnerUln).Include(x => x.ApprenticeshipPriceEpisodes).ToList();
            var ape = apprenticeships.SelectMany(x => x.ApprenticeshipPriceEpisodes).ToList();

            CollectionPeriods = earnings.OrderBy(x => x.Key).Select(periods => new CollectionPeriod
            {
                PeriodName = $"R0{periods.Key}",
                PriceEpisodes = periods.SelectMany(x => x.PriceEpisodes, (period, earning) => MapPriceEpisode(period, earning, paidCommitments, lockedCommitments, apprenticeships)).ToList(),
            }).ToList();
        }

        private static PriceEpisode MapPriceEpisode(
            EarningEventModel earning,
            EarningEventPriceEpisodeModel episode,
            List<PaymentModel> paidCommitments,
            List<DataLockEventModel> lockedCommitments,
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
                        .Where(y => y.CollectionPeriod.Period == earning.CollectionPeriod)
                        .Select(y => new Payment
                        {
                            Id = y.Id,
                            Amount = y.Amount,
                            PriceEpisodeIdentifier = y.PriceEpisodeIdentifier,
                            CollectionPeriod = y.CollectionPeriod,
                            TransactionType = y.TransactionType.ToString(),
                            DeliveryPeriod = y.DeliveryPeriod
                        }).ToList(),
                    DataLocked = lockedCommitments
                        .Where(y => y.CollectionPeriod == earning.CollectionPeriod)
                        .SelectMany(y => y.NonPayablePeriods)
                        .SelectMany(y => y.DataLockEventNonPayablePeriodFailures)
                        .Select(y => new DataLock
                        {
                            Amount = y.DataLockEventNonPayablePeriod.Amount,
                            DataLockErrorCode = y.DataLockFailure,
                            Id = y.Id,
                            DeliveryPeriod = y.DataLockEventNonPayablePeriod.DeliveryPeriod
                        }).ToList(),
                    FrameworkCode = c.FrameworkCode,
                    PathwayCode = c.PathwayCode,
                    StandardCode = c.StandardCode,
                    ProgrammeType = c.ProgrammeType,
                    Ukprn = c.Ukprn,
                    Uln = c.Uln
                }),
                earning.Ukprn,
                earning.LearnerUln,
                earning.LearningAimStandardCode,
                earning.LearningAimFrameworkCode,
                earning.LearningAimProgrammeType,
                earning.LearningAimPathwayCode,
                episode.StartDate);
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