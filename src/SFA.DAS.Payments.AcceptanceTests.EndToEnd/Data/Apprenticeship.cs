using System;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class Apprenticeship
    {
        public string Identifier { get; set; } = "Apprenticeship 1";

        public string LearnerId { get; set; }
        public long Uln { get; set; }

        public string Employer { get; set; }
        public string SendingEmployer { get; set; }
        public long AccountId { get; set; }
        public long SenderAccountId { get; set; }
        public string Provider { get; set; }
        public long Ukprn { get; set; }

        public int Priority { get; set; }
        public long ApprenticeshipId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal AgreedPrice { get; set; }

        public string EffectiveFrom { get; set; }
        public string EffectiveTo { get; set; }
        public string StopEffectiveFrom { get; set; }
        public string VersionId { get; set; }

        public string Status { get; set; }
        public int? FrameworkCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? PathwayCode { get; set; }
        public int? StandardCode { get; set; }
    }

}
