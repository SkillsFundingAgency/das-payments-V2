using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Domain.Models
{
    public class RequiredPaymentEventValidationResult
    {
        public RequiredPaymentEventValidationRules Rule { get; set; }

        public RequiredPaymentEvent RequiredPaymentEventMesage { get; set; }
    }
}