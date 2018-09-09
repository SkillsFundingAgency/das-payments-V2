using System.Collections.Generic;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Domain.Interface
{
    public interface IValidateRequiredPaymentEvent
    {
        IEnumerable<RequiredPaymentEventValidationResult> Validate(ApprenticeshipContractType2RequiredPaymentEvent message);
    }
}