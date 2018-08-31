using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data
{
    public class ContractTypeEarning
    {
        public byte ToPeriod { get; set; }

        public byte FromPeriod { get; set; }

        public short ContractType { get; set; } 

        public string AcademicYear { get; set; }

        public OnProgrammeEarningType EarningType { get; set; }

        public string PriceEpisodeIdentifier { get; set; }

        public DateTime EpisodeStartDate { get; set; }

        public DateTime EpisodeEffectiveTnpStartDate { get; set; }

        public decimal TotalNegotiatedPrice { get; set; }

        public decimal? Learning_1 { get; set; }

        public decimal? Completion_2 { get; set; }

        public decimal? Balancing_3 { get; set; }
    }
}