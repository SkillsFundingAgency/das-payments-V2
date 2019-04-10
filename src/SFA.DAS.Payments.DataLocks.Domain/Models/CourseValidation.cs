using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class CourseValidation
    {
        public DateTime StartDate { get; set; }

        public byte Period { get; set; }

        public string PriceEpisodeIdentifier { get; set; }

        public List<ApprenticeshipModel> Apprenticeships { get; set; }
    }
}