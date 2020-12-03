using System;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData
{
    public class Commitment
    {
        public virtual long Id { get; set; }
        public DateTime EmployerAndProviderApprovedOn { get; set; }
        public short Approvals { get; set; }
    }
}