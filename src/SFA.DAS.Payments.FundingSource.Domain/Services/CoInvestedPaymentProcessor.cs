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

        public CoInvestedPaymentProcessor(IValidateRequiredPaymentEvent validateRequiredPaymentEvent)
        {
            validator = validateRequiredPaymentEvent;
        }

        protected void Validate(CoInvestedPayment message)
        {
            var validationResults = validator.Validate(message);
            if (validationResults.Any()) throw new FundingSourceRequiredPaymentValidationException(JsonConvert.SerializeObject(validationResults));
        }

        public Payment Process(CoInvestedPayment message)
        {
            Validate(message);

            return CreatePayment(message);
        }

        protected abstract Payment CreatePayment(CoInvestedPayment message);
      
    }

}