using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.JobService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsProxyService.Handlers
{
    public class RecordJobMessageProcessingStatusHandler : BaseJobMessageHandler<RecordJobMessageProcessingStatus>
    {
        public RecordJobMessageProcessingStatusHandler(IServiceProxyFactory proxyFactory,
            IPaymentLogger logger) : base(proxyFactory, logger)
        {
        }

        protected override async Task HandleMessage(RecordJobMessageProcessingStatus message, IMessageHandlerContext context, IJobService jobService, CancellationToken cancellationToken)
        {
            await jobService.RecordJobMessageProcessingStatus(message, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}