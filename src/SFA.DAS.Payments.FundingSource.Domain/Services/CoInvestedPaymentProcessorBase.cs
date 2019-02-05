using System.Linq;
using Newtonsoft.Json;
using SFA.DAS.Payments.FundingSource.Domain.Exceptions;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public abstract class CoInvestedPaymentProcessorBase : ICoInvestedPaymentProcessorOld
    {
        private readonly IValidateRequiredPaymentEvent validator;

        protected CoInvestedPaymentProcessorBase(IValidateRequiredPaymentEvent validateRequiredPaymentEvent)
        {
            validator = validateRequiredPaymentEvent;
        }

        private void Validate(RequiredPayment message)
        {
            var validationResults = validator.Validate(message);
            if (validationResults.Any()) throw new FundingSourceRequiredPaymentValidationException(JsonConvert.SerializeObject(validationResults));
        }

        public FundingSourcePayment Process(RequiredPayment message)
        {
            Validate(message);

            var fundingSourcePayment = CreatePayment(message);
            return fundingSourcePayment;
        }

        protected abstract FundingSourcePayment CreatePayment(RequiredPayment message);
    }
}