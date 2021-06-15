using System;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public class CalculatedRequiredLevyAmount : CalculatedRequiredOnProgrammeAmount, ILeafLevelMessage
    {
        public int Priority { get; set; }
        public string AgreementId { get; set; }
        public DateTime? AgreedOnDate { get; set; }
    }
}