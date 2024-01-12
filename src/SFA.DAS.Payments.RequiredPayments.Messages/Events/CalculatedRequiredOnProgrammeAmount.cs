using SFA.DAS.Payments.Messages.Common;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public abstract class CalculatedRequiredOnProgrammeAmount : PeriodisedRequiredPaymentEvent, IMonitoredMessage
    {
        public decimal SfaContributionPercentage { get; set; }
        public OnProgrammeEarningType OnProgrammeEarningType { get; set; }
        public override TransactionType TransactionType => (TransactionType)OnProgrammeEarningType; 
    }
}