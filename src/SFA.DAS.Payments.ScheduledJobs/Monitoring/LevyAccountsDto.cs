using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring
{
    public class LevyAccountsDto
    {
        public LevyAccountModel DasLevyAccount { get; set; }
        public LevyAccountModel PaymentsLevyAccount { get; set; }
    }
}
