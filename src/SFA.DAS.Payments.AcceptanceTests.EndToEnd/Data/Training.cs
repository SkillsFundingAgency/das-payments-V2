using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class Training
    {
        public long Uln { get; set; }
        public int Priority { get; set; }
        public string StartDate { get; set; }
        public string PlannedDuration { get; set; }
        public decimal TotalTrainingPrice { get; set; }
        public decimal TotalAssessmentPrice { get; set; }
        public string ActualDuration { get; set; }
        public int ProgrammeType { get; set; }
        public string CompletionStatus { get; set; }
        public decimal SfaContributionPercentage { get; set; }
    }
}
