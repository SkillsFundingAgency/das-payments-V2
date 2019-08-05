using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class ApprenticeshipPauseModel
    {
        public long Id { get; set; }
        public long ApprenticeshipId { get; set; }
        public DateTime PauseDate   { get; set; }
        public DateTime? ResumeDate { get; set; }
    }
}
