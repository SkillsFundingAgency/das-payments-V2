using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class DataLockValidationModel
    {
        public PriceEpisode PriceEpisode { get; set; }
        public EarningPeriod EarningPeriod { get; set; }
        public ApprenticeshipModel Apprenticeship { get; set; }
        public LearningAim Aim { get; set; }
        public int AcademicYear { get; set; }
        public OnProgrammeEarningType TransactionType { get; set; }
    }
}
