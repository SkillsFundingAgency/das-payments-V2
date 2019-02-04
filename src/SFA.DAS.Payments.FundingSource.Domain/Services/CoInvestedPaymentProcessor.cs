using System.Collections.Generic;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using System.Linq;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class CoInvestedPaymentProcessor : ICoInvestedPaymentProcessor
    {
        private readonly IPaymentProcessor employerCoInvestedPaymentProcessor;
        private readonly IPaymentProcessor sfaCoInvestedPaymentProcessor;

        public CoInvestedPaymentProcessor(IPaymentProcessor employerCoInvestedPaymentProcessor, IPaymentProcessor sfaCoInvestedPaymentProcessor)
        {
            this.employerCoInvestedPaymentProcessor = employerCoInvestedPaymentProcessor;
            this.sfaCoInvestedPaymentProcessor = sfaCoInvestedPaymentProcessor;
        }

        public IReadOnlyList<FundingSourcePayment> Process(RequiredPayment requiredPayment)
        {
            return employerCoInvestedPaymentProcessor.Process(requiredPayment)
                .Concat(sfaCoInvestedPaymentProcessor.Process(requiredPayment)).ToList();
        }
    }
}