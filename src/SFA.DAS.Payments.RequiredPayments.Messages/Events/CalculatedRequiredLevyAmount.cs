using System;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public class CalculatedRequiredLevyAmount : CalculatedRequiredOnProgrammeAmount, ILeafLevelMessage
    {
        public int Priority { get; set; }
        public string AgreementId { get; set; }
        public DateTime? AgreedOnDate { get; set; }
        public FundingPlatformType FundingPlatformType { get; set; } = FundingPlatformType.SubmitLearnerData;
    }
}