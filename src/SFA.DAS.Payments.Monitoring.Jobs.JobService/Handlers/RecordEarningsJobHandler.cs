using Microsoft.ServiceFabric.Services.Remoting.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.JobService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsService.Handlers
{
    public class RecordEarningsJobHandler: BaseJobMessageHandler<RecordEarningsJob>
    {
        public RecordEarningsJobHandler(IServiceProxyFactory proxyFactory, IPaymentLogger logger): base(proxyFactory,logger)
        {
        }

        protected override Task HandleMessage(RecordEarningsJob message, IMessageHandlerContext context, IJobService jobService,
            CancellationToken cancellationToken)
        {
            return jobService.RecordEarningsJob(message, cancellationToken);
        }
    }
}