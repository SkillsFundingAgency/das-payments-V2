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
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class EarningEventsService : StatelessService
    {
        private readonly ILifetimeScope lifetimeScope;
        private readonly IJobContextManager<JobContextMessage> jobContextManager;
        private readonly IPaymentLogger logger;

        public EarningEventsService(StatelessServiceContext context, ILifetimeScope lifetimeScope, IJobContextManager<JobContextMessage> jobContextManager, IPaymentLogger logger)
            : base(context)
        {
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            this.jobContextManager = jobContextManager;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new List<ServiceInstanceListener>
            {
                new ServiceInstanceListener(context =>lifetimeScope.Resolve<IEndpointCommunicationListener>())
            };
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var initialised = false;

            try
            {
                logger.LogInfo("Earning Events Stateless Service Start");

                jobContextManager.OpenAsync(cancellationToken);
                initialised = true;
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (Exception exception) when (!(exception is TaskCanceledException))
            {
                // Ignore, as an exception is only really thrown on cancellation of the token.
                logger.LogError("Reference Data Stateless Service Exception", exception);
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