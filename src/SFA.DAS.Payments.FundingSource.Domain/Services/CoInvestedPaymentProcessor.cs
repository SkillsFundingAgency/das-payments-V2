using Newtonsoft.Json;
using SFA.DAS.Payments.FundingSource.Domain.Exceptions;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
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

        protected void Validate(ApprenticeshipContractType2RequiredPaymentEvent message)
        {
            var validationResults = validator.Validate(message);
            if (validationResults.Any()) throw new FundingSourceRequiredPaymentValidationException(JsonConvert.SerializeObject(validationResults));
        }

        public CoInvestedFundingSourcePaymentEvent Process(ApprenticeshipContractType2RequiredPaymentEvent message)
        {
            Validate(message);

            return CreatePayment(message);
        }

        protected abstract CoInvestedFundingSourcePaymentEvent CreatePayment(ApprenticeshipContractType2RequiredPaymentEvent message);
      
    }

}