using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Messages.Events
{
    public abstract class JobStatusEvent
    {
    public long JobId { get; set; }
    }
}