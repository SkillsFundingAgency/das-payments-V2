using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Serialization.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Monitoring.JobStatus.Client;

namespace SFA.DAS.Payments.EarningEvents.Application.Handlers
{
    public class JobContextMessageHandler : IMessageHandler<JobContextMessage>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IKeyValuePersistenceService redisService;
        private readonly IJsonSerializationService serializationService;
        private readonly IEndpointInstanceFactory factory;
        private readonly IProviderEarningsJobStatusClient jobStatusClient;


        public JobContextMessageHandler(IPaymentLogger paymentLogger,
            IKeyValuePersistenceService redisService,
            IJsonSerializationService serializationService,
            IEndpointInstanceFactory factory,
            IProviderEarningsJobStatusClient jobStatusClient
            )
        {
            this.paymentLogger = paymentLogger;
            this.redisService = redisService;
            this.serializationService = serializationService;
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.jobStatusClient = jobStatusClient ?? throw new ArgumentNullException(nameof(jobStatusClient));
        }


        public async Task<bool> HandleAsync(JobContextMessage message, CancellationToken cancellationToken)
        {
            paymentLogger.LogDebug($"Processing Earning Event Service event for Job Id : {message.JobId}");

            try
            {
                var fm36Json = await redisService.GetAsync(message.KeyValuePairs["FundingFm36Output"].ToString(), cancellationToken);
                var fm36Output = serializationService.Deserialize<FM36Global>(fm36Json);
                var commands = new List<(DateTimeOffset StartTime, Guid Ids)>();
                foreach (var learner in fm36Output.Learners)
                {
                    try
                    {
                        var learnerCommand = new ProcessLearnerCommand
                        {
                            JobId = message.JobId,
                            Learner = learner,
                            RequestTime = DateTimeOffset.UtcNow,
                            IlrSubmissionDateTime = message.SubmissionDateTimeUtc,
                            CollectionYear = fm36Output.Year,
                            CollectionPeriod = 1, //TODO: Need to get the collection period from DC!!!!
                            Ukprn = fm36Output.UKPRN
                        };
                        var endpointInstance = await factory.GetEndpointInstance();
                        await endpointInstance.SendLocal(learnerCommand);
                        commands.Add((DateTimeOffset.UtcNow, learnerCommand.CommandId));
                        paymentLogger.LogInfo(
                            $"Successfully sent ProcessLearnerCommand JobId: {learnerCommand.JobId}, Ukprn: {fm36Output.UKPRN}, LearnRefNumber: {learner.LearnRefNumber}, SubmissionTime: {message.SubmissionDateTimeUtc}");
                    }
                    catch (Exception ex)
                    {
                        paymentLogger.LogError("Error publishing the event: EarningEvent", ex);
                        throw;
                    }
                }

                await jobStatusClient.StartJob(message.JobId, fm36Output.UKPRN, message.SubmissionDateTimeUtc, short.Parse(fm36Output.Year), 1, commands);
                paymentLogger.LogInfo($"Successfully processed ILR Submission. Job Id: {message.JobId}, Ukprn: {fm36Output.UKPRN}, Submission Time: {message.SubmissionDateTimeUtc}");
                return true;
            }
            catch (Exception ex)
            {
                paymentLogger.LogError("Error while handling EarningService event", ex);
                throw;
            }
        }
    }
}
