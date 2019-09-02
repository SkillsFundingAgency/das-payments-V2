using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Client.Infrastructure;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.JobsService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public interface IEarningsJobClient
    {
        Task StartJob(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages, DateTimeOffset startTime);
    }

    public class EarningsJobClient : IEarningsJobClient
    {

        private readonly IMessageSession messageSession;
        private readonly IPaymentLogger logger;
        private readonly IActorProxyFactory proxyFactory;

        public EarningsJobClient(IMessageSession messageSession, IPaymentLogger logger, IActorProxyFactory proxyFactory)
        {
            this.messageSession = messageSession ?? throw new ArgumentNullException(nameof(messageSession));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
        }

        public async Task StartJob(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages, DateTimeOffset startTime)
        {
            logger.LogVerbose($"Sending request to record start of earnings job. Job Id: {jobId}, Ukprn: {ukprn}");
            var providerEarningsEvent = new RecordEarningsJob
            {
                StartTime = startTime,
                JobId = jobId,
                Ukprn = ukprn,
                IlrSubmissionTime = ilrSubmissionTime,
                CollectionYear = collectionYear,
                CollectionPeriod = collectionPeriod,
                GeneratedMessages = new List<GeneratedMessage>(),
                LearnerCount = generatedMessages.Count
            };
            try
            {
                var actorId = new ActorId(jobId.ToString());
                var actor = proxyFactory.CreateActorProxy<IJobsService>(new Uri(ServiceUris.JobsServiceUri), actorId);
                await actor.RecordEarningsJob(providerEarningsEvent, CancellationToken.None).ConfigureAwait(false);
                logger.LogDebug($"Sent request to record start of earnings job. Job Id: {jobId}, Ukprn: {ukprn}");
            }
            catch (Exception e)
            {
                logger.LogWarning($"Failed to invoke monitoring actor using remoting when trying to create job.  Falling back to messaging notification for Job: {jobId}, Ukprn: {ukprn}.");
                await messageSession.Send(providerEarningsEvent).ConfigureAwait(false);
            }
        }
    }
}