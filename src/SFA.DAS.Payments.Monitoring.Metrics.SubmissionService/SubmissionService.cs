using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.Monitoring.Metrics.SubmissionService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class SubmissionService : StatelessService
    {
        private IStatelessEndpointCommunicationListener listener;
        private readonly ILifetimeScope lifetimeScope;
        private readonly IPaymentLogger logger;

        public SubmissionService(StatelessServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger logger)
            : base(context)
        {
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            logger.LogInfo("Creating Service Instance Listeners For SubmissionService");


            return new List<ServiceInstanceListener>
            {
                new ServiceInstanceListener(context =>listener = lifetimeScope.Resolve<IStatelessEndpointCommunicationListener>())
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                var jobs = new List<(long Job, long Ukprn, byte collectionPeriod, DateTime ildSubmissionDate)>()
                {
                    //(68217,10033440,8,DateTime.Parse("2020-03-23 17:10:11.093")),
                };
                var factory = lifetimeScope.Resolve<IEndpointInstanceFactory>();
                var endpoint = await factory.GetEndpointInstance();
                foreach (var job in jobs)
                {
                    await endpoint.Publish(new SubmissionJobSucceeded
                    {
                        CollectionPeriod = job.collectionPeriod,
                        JobId = job.Job,
                        Ukprn = job.Ukprn,
                        AcademicYear = 1920,
                        IlrSubmissionDateTime = job.ildSubmissionDate
                    }).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                
                logger.LogWarning($"Failed to publish jobs. Error: {e.Message}");
            }

        }
    }
}
