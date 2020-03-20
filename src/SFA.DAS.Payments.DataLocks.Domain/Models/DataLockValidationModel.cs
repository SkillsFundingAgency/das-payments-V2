using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class DataLockValidationModel
    {
        public PriceEpisode PriceEpisode { get; set; }
        public EarningPeriod EarningPeriod { get; set; }
        public ApprenticeshipModel Apprenticeship { get; set; }
        public LearningAim Aim { get; set; }
        public int AcademicYear { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
