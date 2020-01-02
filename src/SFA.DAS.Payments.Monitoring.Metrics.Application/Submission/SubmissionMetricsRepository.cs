using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Model;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Submission
{
    public interface ISubmissionMetricsRepository
    {
        Task<List<TransactionTypeAmounts>> GetDasEarnings(long ukprn, long jobId);
    }

    public class SubmissionMetricsRepository: ISubmissionMetricsRepository
    {
        private readonly IPaymentsDataContext paymentsDataContext;

        public SubmissionMetricsRepository(IPaymentsDataContext paymentsDataContext)
        {
            this.paymentsDataContext = paymentsDataContext ?? throw new ArgumentNullException(nameof(paymentsDataContext));
        }

        public async Task<List<TransactionTypeAmounts>> GetDasEarnings(long ukprn, long jobId)
        {

            var transactionAmounts = await paymentsDataContext.EarningEventPeriod
                .AsNoTracking()
                .Where(ee => ee.EarningEvent.Ukprn == ukprn && ee.EarningEvent.JobId == jobId)
                //.Select(eep => new { eep.Amount, eep.EarningEvent.ContractType, eep.TransactionType})
                .GroupBy(eep => new { eep.EarningEvent.ContractType, eep.TransactionType })
                .Select(group => new
                {
                    ContractType = group.Key.ContractType,
                    TransactionType = group.Key.TransactionType,
                    Amount = group.Sum(x => x.Amount)
                })
                .ToListAsync();

            return transactionAmounts
                .GroupBy(x => x.ContractType)
                .Select(group => new TransactionTypeAmounts
                {
                    ContractType = group.Key,
                    TransactionType1 = group.Where(x => x.TransactionType == TransactionType.Learning).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType2 = group.Where(x => x.TransactionType == TransactionType.Balancing).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType3 = group.Where(x => x.TransactionType == TransactionType.Completion).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType4 = group.Where(x => x.TransactionType == TransactionType.First16To18EmployerIncentive).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType5 = group.Where(x => x.TransactionType == TransactionType.First16To18ProviderIncentive).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType6 = group.Where(x => x.TransactionType == TransactionType.Second16To18EmployerIncentive).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType7 = group.Where(x => x.TransactionType == TransactionType.Second16To18ProviderIncentive).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType8 = group.Where(x => x.TransactionType == TransactionType.OnProgramme16To18FrameworkUplift).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType9 = group.Where(x => x.TransactionType == TransactionType.Completion16To18FrameworkUplift).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType10 = group.Where(x => x.TransactionType == TransactionType.Balancing16To18FrameworkUplift).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType11 = group.Where(x => x.TransactionType == TransactionType.FirstDisadvantagePayment).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType12 = group.Where(x => x.TransactionType == TransactionType.SecondDisadvantagePayment).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType13 = group.Where(x => x.TransactionType == TransactionType.OnProgrammeMathsAndEnglish).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType14 = group.Where(x => x.TransactionType == TransactionType.BalancingMathsAndEnglish).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType15 = group.Where(x => x.TransactionType == TransactionType.LearningSupport).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType16 = group.Where(x => x.TransactionType == TransactionType.CareLeaverApprenticePayment).Sum(x => (decimal?)x.Amount) ?? 0,
                })
                .ToList();

            //TODO: fix the EarningEventPeriodModel, Add an EarningEvent navigation property
            //paymentsDataContext.EarningEvent
            //    .AsNoTracking()
            //    .Where(ee => ee.Ukprn== ukprn && ee.JobId == jobId )
            //    .SelectMany(ee => ee.Periods, (model, periodModel) =>  new { model.ContractType, Period = periodModel } )
            //    .Where(model => model.Period.Amount != 0)
            //    .Select(model => new { model.ContractType, model.Period.TransactionType, model.Period.Amount })

            //    .GroupBy(model => model.ContractType)
            //    .Select( group => new
            //    {
            //        ContarctType =  group.Key,
            //        TransactionType1 = group.Sum(x => x.TransactionType)
            //    }})

        }
    }
}