using System;
using System.Fabric;
using System.Fabric.Description;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

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
            logger.LogInfo($"Received period end started event. Details: {message.ToJson()}");

            var serviceName = "fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric/SFA.DAS.Payments.DataLocks.ApprovalsService";
            
            var fabricClient = new FabricClient();
            var serviceDescription = new DeleteServiceDescription(new Uri(serviceName)) 
            {
                ForceDelete = true,
            };

            try
            {
                await fabricClient.ServiceManager.DeleteServiceAsync(serviceDescription);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to delete the new service. {e}");
            }
        }
    }
}