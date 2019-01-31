using Newtonsoft.Json;
using SFA.DAS.Payments.FundingSource.Domain.Exceptions;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using System.Linq;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public abstract class CoInvestedPaymentProcessor: ICoInvestedPaymentProcessor
    {
        private readonly IValidateRequiredPaymentEvent validator;

        protected CoInvestedPaymentProcessor(IValidateRequiredPaymentEvent validateRequiredPaymentEvent)
        {
            validator = validateRequiredPaymentEvent;
        }

        private void Validate(RequiredCoInvestedPayment message)
        {
            var validationResults = validator.Validate(message);
            if (validationResults.Any()) throw new FundingSourceRequiredPaymentValidationException(JsonConvert.SerializeObject(validationResults));
        }

        public FundingSourcePayment Process(RequiredPayment message)
        {
            Validate((RequiredCoInvestedPayment) message);

            var fundingSourcePayment = CreatePayment((RequiredCoInvestedPayment) message);
            message.AmountFunded += fundingSourcePayment.AmountDue;
            return fundingSourcePayment;
        }

        protected abstract FundingSourcePayment CreatePayment(RequiredCoInvestedPayment message);
    }
}