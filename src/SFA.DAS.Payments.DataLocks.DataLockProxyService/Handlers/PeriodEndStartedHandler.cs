using System;
using System.Fabric;
using System.Fabric.Description;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;
using SFA.DAS.Payments.ServiceFabric.Core.Constants;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class PeriodEndStartedHandler: IHandleMessages<PeriodEndStartedEvent>
    {
        private readonly IPaymentLogger logger;
        
        public PeriodEndStartedHandler(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PeriodEndStartedEvent message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Received period end started event. " +
                            $"JobId: {message.JobId}, " +
                            $"Collection Period: {message.CollectionPeriod}");

            var fabricClient = new FabricClient();
            var serviceDescription = new DeleteServiceDescription(new Uri(ServiceNames.DatalockApprovalsService)) 
            {
                ForceDelete = true,
            };

            await fabricClient.ServiceManager.DeleteServiceAsync(serviceDescription);

            logger.LogInfo($"Finished period end started handler. " +
                           $"JobId: {message.JobId}, " +
                           $"Collection Period: {message.CollectionPeriod}");
        }
    }
}