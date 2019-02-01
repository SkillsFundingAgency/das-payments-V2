using System.Collections.Generic;
using Newtonsoft.Json;
using SFA.DAS.Payments.FundingSource.Domain.Exceptions;
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

    public abstract class CoInvestedPaymentProcessorBase
    {
        private readonly IValidateRequiredPaymentEvent validator;

        protected CoInvestedPaymentProcessorBase(IValidateRequiredPaymentEvent validateRequiredPaymentEvent)
        {
            validator = validateRequiredPaymentEvent;
        }

        private void Validate(RequiredCoInvestedPayment message)
        {
            var validationResults = validator.Validate(message);
            if (validationResults.Any()) throw new FundingSourceRequiredPaymentValidationException(JsonConvert.SerializeObject(validationResults));
        }

        public FundingSourcePayment Process(RequiredCoInvestedPayment message)
        {
            Validate(message);

            var fundingSourcePayment = CreatePayment(message);
            return fundingSourcePayment;
        }

        protected abstract FundingSourcePayment CreatePayment(RequiredPayment message);
    }
}