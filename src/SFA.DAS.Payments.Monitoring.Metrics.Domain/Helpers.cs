using SFA.DAS.Payments.Monitoring.Metrics.Model;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain
{
    public static class Helpers
    {
        public static decimal GetPercentage(decimal amount, decimal total) => amount == total ? 100 : total > 0 ? (amount / total) * 100 : 0;

        public static decimal GetDefaultPercentage(this ContractTypeAmountsVerbose contactTypeAmountsVerbose)
        {
            var percentageContractType1 = contactTypeAmountsVerbose.PercentageContractType1;
            var percentageContractType2 = contactTypeAmountsVerbose.PercentageContractType2;
            return percentageContractType1 + percentageContractType2  == 0 ? 0 :
                (percentageContractType1 + percentageContractType2) / (percentageContractType1==0 ||  percentageContractType2==0 ? 1 : 2) ;
        }

    }
}