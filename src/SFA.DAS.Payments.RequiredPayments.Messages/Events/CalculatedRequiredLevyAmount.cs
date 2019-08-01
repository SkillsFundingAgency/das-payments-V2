using System;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public class CalculatedRequiredLevyAmount : CalculatedRequiredOnProgrammeAmount
    {
        public int Priority { get; set; }
        public long ApprenticeshipId { get; set; }
        public long ApprenticeshipPriceEpisodeId { get; set; }
        public string AgreementId { get; set; }
        public DateTime? AgreedOnDate { get; set; }
    }
}