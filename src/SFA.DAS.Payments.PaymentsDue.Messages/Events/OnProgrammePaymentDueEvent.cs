using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.PaymentsDue.Messages.Events
{
    public class OnProgrammePaymentDueEvent : PaymentDueEvent
    {
        public OnProgrammeEarningType OnProgrammeEarningType
        {
            get => (OnProgrammeEarningType)TransactionType;
            set => TransactionType = (int)value;
        }
    }
}