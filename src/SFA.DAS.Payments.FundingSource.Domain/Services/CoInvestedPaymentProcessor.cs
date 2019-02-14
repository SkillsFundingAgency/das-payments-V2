using System.Collections.Generic;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class CoInvestedPaymentProcessor : ICoInvestedPaymentProcessor
    {
        private readonly IEmployerCoInvestedPaymentProcessor employerCoInvestedPaymentProcessor;
        private readonly ISfaCoInvestedPaymentProcessor sfaCoInvestedPaymentProcessor;

        public CoInvestedPaymentProcessor(IEmployerCoInvestedPaymentProcessor employerCoInvestedPaymentProcessor, ISfaCoInvestedPaymentProcessor sfaCoInvestedPaymentProcessor)
        {
            this.employerCoInvestedPaymentProcessor = employerCoInvestedPaymentProcessor;
            this.sfaCoInvestedPaymentProcessor = sfaCoInvestedPaymentProcessor;
        }

        public IReadOnlyList<FundingSourcePayment> Process(RequiredPayment requiredPayment)
        {
            return new []{ employerCoInvestedPaymentProcessor.Process(requiredPayment),
                sfaCoInvestedPaymentProcessor.Process(requiredPayment)};
        }
    }
}