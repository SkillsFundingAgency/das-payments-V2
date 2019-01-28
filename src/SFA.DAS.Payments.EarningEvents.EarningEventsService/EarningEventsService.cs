using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.EarningEvents.EarningEventsService
{
    public class EarningEventsService : StatelessService
    {
        private readonly ILifetimeScope lifetimeScope;
        private IJobContextManager<JobContextMessage> jobContextManager;
        private readonly IPaymentLogger logger;

        public EarningEventsService(StatelessServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger logger)
            : base(context)
        {
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new List<ServiceInstanceListener>
            {
                new ServiceInstanceListener(context =>lifetimeScope.Resolve<IStatelessEndpointCommunicationListener>())
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var initialised = false;

            try
            {
                logger.LogDebug("Starting the Earning Events service.");
                jobContextManager = lifetimeScope.Resolve<IJobContextManager<JobContextMessage>>();
                jobContextManager.OpenAsync(cancellationToken);
                initialised = true;
                logger.LogInfo("Started the Earning Events service.");
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (Exception exception) when (!(exception is TaskCanceledException))
            {
                // Ignore, as an exception is only really thrown on cancellation of the token.
                logger.LogError($"Reference Data Stateless Service Exception. Error: {exception}.", exception);
            }
            finally
            {
                if (initialised)
                {
                    logger.LogInfo("Earning Events Stateless Service End");
                    await jobContextManager.CloseAsync();
                }
            }
        }
    }
}