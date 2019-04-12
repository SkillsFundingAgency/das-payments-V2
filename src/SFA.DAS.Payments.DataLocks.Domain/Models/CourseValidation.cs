using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class CourseValidation
    {
        public PriceEpisode PriceEpisode { get; set; }
        public byte Period { get; set; }
        public List<ApprenticeshipModel> Apprenticeships { get; set; }
    }
}