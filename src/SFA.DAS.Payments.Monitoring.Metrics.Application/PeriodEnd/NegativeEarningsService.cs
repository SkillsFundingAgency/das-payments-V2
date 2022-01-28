using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.PeriodEnd
{
    public interface INegativeEarningsService
    {
        NegativeEarningsContractTypeAmounts CalculateNegativeEarningsForProvider(
            List<ProviderLearnerNegativeEarningsTotal> providerLearnerNegativeEarnings,
            List<ProviderLearnerContractTypeAmounts> providerLearnerPayments,
            List<ProviderLearnerDataLockEarningsTotal> providerLearnerDataLocks);
    }

    public class NegativeEarningsService : INegativeEarningsService
    {
        public NegativeEarningsContractTypeAmounts CalculateNegativeEarningsForProvider(List<ProviderLearnerNegativeEarningsTotal> providerLearnerNegativeEarnings,
            List<ProviderLearnerContractTypeAmounts> providerLearnerPayments, List<ProviderLearnerDataLockEarningsTotal> providerLearnerDataLocks)
        {
            var result = new NegativeEarningsContractTypeAmounts();

            if (providerLearnerNegativeEarnings == null || providerLearnerNegativeEarnings.Count == 0) return result;

            var distinctLearnerUlns = providerLearnerNegativeEarnings.Select(x => x.Uln).Distinct().ToList();

            foreach (var uln in distinctLearnerUlns)
            {
                //if learner has payments, then do not add negative earnings to provider summary
                var learnerPayments = providerLearnerPayments?.Where(x => x.LearnerUln == uln && x.Total > 0m).ToList();
                if (learnerPayments != null) continue;

                //if learner has data locks, then do not add negative earnings to provider summary
                var learnerDataLocks = providerLearnerDataLocks?.Where(x => x.LearnerUln == uln).ToList();
                if (learnerDataLocks != null) continue;

                //add negative earnings to provider summary
                var learnerNegativeEarnings = providerLearnerNegativeEarnings.Where(x => x.Uln == uln).ToList();
                learnerNegativeEarnings.ForEach(x =>
                {
                    switch (x.ContractType)
                    {
                        case ContractType.Act1:
                            result.ContractType1 += x.NegativeEarningsTotal;
                            break;

                        case ContractType.Act2:
                            result.ContractType2 += x.NegativeEarningsTotal;
                            break;
                    }
                });
            }

            return result;
        }
    }
}