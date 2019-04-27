using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public abstract class OnProgrammePeriodValidationResult
    {
        public ApprenticeshipModel Apprenticeship { get; set; }
        public EarningPeriod Period { get; set; }
    }
}