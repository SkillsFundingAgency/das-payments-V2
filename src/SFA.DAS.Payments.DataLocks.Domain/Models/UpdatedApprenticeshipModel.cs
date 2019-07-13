using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class UpdatedApprenticeshipModel
    {
        public long ApprenticeshipId { get; set; }
        public DateTime AgreedOnDate { get; set; }
        public long? StandardCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? FrameworkCode { get; set; }
        public int? PathwayCode { get; set; }

        public List<ApprenticeshipPriceEpisodeModel> ApprenticeshipPriceEpisodes { get; set; }
    }
}