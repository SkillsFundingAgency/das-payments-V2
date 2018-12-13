using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Serialization.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.Handlers
{
    public class JobContextMessageHandler: IMessageHandler<JobContextMessage>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IFileService azureFileService;
        private readonly IJsonSerializationService serializationService;
        private readonly IEndpointInstanceFactory factory;
        

        public JobContextMessageHandler(IPaymentLogger paymentLogger,
            IFileService azureFileService,
            IJsonSerializationService serializationService,
            IEndpointInstanceFactory factory)
        {
            this.paymentLogger = paymentLogger;
            this.azureFileService = azureFileService;
            this.serializationService = serializationService;
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
       

        public async Task<bool> HandleAsync(JobContextMessage message, CancellationToken cancellationToken)
        {
            paymentLogger.LogDebug($"Processing Earning Event Service event for Job Id : {message.JobId}");
         
            try
            {
                FM36Global fm36Output;

                using (var stream = await azureFileService.OpenReadStreamAsync(message.KeyValuePairs["FundingFm36Output"].ToString(), message.KeyValuePairs["Container"].ToString(), cancellationToken))
                {
                    fm36Output = serializationService.Deserialize<FM36Global>(stream);
                }

                var collectionPeriod = int.Parse(message.KeyValuePairs["ReturnPeriod"].ToString());

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
                            CollectionPeriod = collectionPeriod,
                            Ukprn = fm36Output.UKPRN
                        };
                        var endpointInstance = await factory.GetEndpointInstance();
                        await endpointInstance.SendLocal(learnerCommand);

                        paymentLogger.LogInfo(
                            $"Successfully sent ProcessLearnerCommand JobId: {learnerCommand.JobId}, Ukprn: {fm36Output.UKPRN}, LearnRefNumber: {learner.LearnRefNumber}, SubmissionTime: {message.SubmissionDateTimeUtc}");
                    }
                    catch (Exception ex)
                    {
                        paymentLogger.LogError("Error publishing the event: EarningEvent", ex);
                        throw;
                    }
                }

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
