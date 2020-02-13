using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{

    public interface IPeriodEndJobStatusService: IJobStatusService{ }

    public class PeriodEndJobStatusService: JobStatusService, IPeriodEndJobStatusService
    {
        public PeriodEndJobStatusService(IJobStorageService jobStorageService, IPaymentLogger logger, ITelemetry telemetry, IJobStatusEventPublisher eventPublisher, IJobServiceConfiguration config) : base(jobStorageService, logger, telemetry, eventPublisher, config)
        {
        }
    }
}