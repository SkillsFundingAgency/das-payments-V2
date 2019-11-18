using System;
using System.Fabric;
using System.Fabric.Description;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
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

        public Task Handle(PeriodEndStartedEvent message, IMessageHandlerContext context)
        {
            logger.LogInfo($"Received period end started event. Details: {message.ToJson()}");

            long iterations = 0;
            var appName = "SFA.DAS.Payments.DataLocks.ApprovalsServiceType";
            var serviceName = "ApprovalsService";
            var serviceType = "ApprovalsServiceType";

            var fabricClient = new FabricClient();
            var partitionDescription = new SingletonPartitionSchemeDescription();
            var serviceDescription = new StatelessServiceDescription()
            {
                ApplicationName = new Uri(appName),
                PartitionSchemeDescription = partitionDescription,
                ServiceName = new Uri(serviceName),
                ServiceTypeName = serviceType
            };

            // Create the service instance.  If the service is declared as a default service in the ApplicationManifest.xml,
            // the service instance is already running and this call will fail.
            try
            {
                await fabricClient.ServiceManager.DeleteServiceAsync(serviceDescription);
            }
            catch (AggregateException ae)
            {
                Console.WriteLine($"Failed to deploy the new service. {ae}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to deploy the new service. {e}");
            }



            return Task.CompletedTask;
        }
    }
}