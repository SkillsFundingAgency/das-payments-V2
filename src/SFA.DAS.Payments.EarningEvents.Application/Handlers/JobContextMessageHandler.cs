using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Serialization.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.Handlers
{
    public class JobContextMessageHandler: IMessageHandler<JobContextMessage>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IKeyValuePersistenceService redisService;
        private readonly IJsonSerializationService serializationService;
        private readonly IMessageSession session;

        public JobContextMessageHandler(IPaymentLogger paymentLogger, 
            IKeyValuePersistenceService redisService,
            IJsonSerializationService serializationService,
            IMessageSession session)
        {
            this.paymentLogger = paymentLogger;
            this.redisService = redisService;
            this.serializationService = serializationService;
            this.session = session;
        }
       

        public async Task<bool> HandleAsync(JobContextMessage message, CancellationToken cancellationToken)
        {
            paymentLogger.LogDebug($"Processing Earning Event Service event for Job Id : {message.JobId}");
         
            try
            {
                var fm36Json = await redisService
                    .GetAsync(message.KeyValuePairs["FundingFm36Output"].ToString(), cancellationToken);

                var fm36Output = serializationService.Deserialize<FM36Global>(fm36Json);

                foreach (var learner in fm36Output.Learners)
                {
                    try
                    {
                        var learnerCommand = new ProcessLearnerCommand
                        {
                            JobId = message.JobId.ToString(),
                            Learner = learner,
                            RequestTime = DateTimeOffset.UtcNow,
                            SubmissionTime = message.SubmissionDateTimeUtc
                        };

                        await session.SendLocal(learnerCommand);

                        paymentLogger.LogInfo(
                            $"Successfully sent ProcessLearnerCommand JobId: {learnerCommand.JobId}, Ukprn: {fm36Output.UKPRN}, LearnRefNumber: {learner.LearnRefNumber}, SubmissionTime: {message.SubmissionDateTimeUtc}");
                    }
                    catch (Exception ex)
                    {
                        paymentLogger.LogError("Error publishing the event: EarningEvent", ex);
                        throw;
                    }
                }

                paymentLogger.LogInfo($"Successfully processed Earning event for Job Id {message.JobId}");

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
