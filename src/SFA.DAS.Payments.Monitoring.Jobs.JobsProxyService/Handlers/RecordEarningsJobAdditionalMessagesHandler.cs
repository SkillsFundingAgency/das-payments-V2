using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.JobService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsProxyService.Handlers
{
    public class RecordEarningsJobAdditionalMessagesHandler : BaseJobMessageHandler<RecordEarningsJobAdditionalMessages>
    {
        public RecordEarningsJobAdditionalMessagesHandler(IServiceProxyFactory proxyFactory, IPaymentLogger logger) : base(proxyFactory, logger)
        {
        }

        protected override async Task HandleMessage(RecordEarningsJobAdditionalMessages message, IMessageHandlerContext context, IJobService jobService,
            CancellationToken cancellationToken)
        {
            await jobService.RecordEarningsJobAdditionalMessages(message, cancellationToken).ConfigureAwait(false);
        }
    }
}