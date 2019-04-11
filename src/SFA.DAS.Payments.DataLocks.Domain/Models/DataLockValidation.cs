using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class DataLockValidation
    {
        public long Uln { get; set; }
        public PriceEpisode PriceEpisode { get; set; }
        public EarningPeriod EarningPeriod { get; set; }
        public List<ApprenticeshipModel> Apprenticeships { get; set; }
    }
}
