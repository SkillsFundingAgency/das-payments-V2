using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.EarningEvents.Domain;
using SFA.DAS.Payments.EarningEvents.Domain.Interfaces;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.EarningEvents.ProcessLearnerService.Interfaces;


namespace SFA.DAS.Payments.EarningEvents.ProcessLearnerService
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    public class ProcessLearnerService : Actor, IProcessLearnerService
    {
        private readonly ILearnerSubmissionProcessor learnerSubmissionProcessor;
        private readonly IPaymentLogger logger;
        private readonly IDuplicateLearnerService duplicateLearnerService;
        private readonly ITelemetry telemetry;

        public ProcessLearnerService(
            ActorService actorService,
            ActorId actorId,
            ILearnerSubmissionProcessor learnerSubmissionProcessor, 
            IPaymentLogger logger,
            IDuplicateLearnerService duplicateLearnerService,
            ITelemetry telemetry
            ) : base(actorService, actorId)
        {
            this.learnerSubmissionProcessor = learnerSubmissionProcessor ?? throw new ArgumentNullException(nameof(learnerSubmissionProcessor));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.duplicateLearnerService = duplicateLearnerService ?? throw new ArgumentNullException(nameof(duplicateLearnerService));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        protected override async Task OnActivateAsync()
        {
            var operationName = $"{nameof(ProcessLearnerService)}.OnActivateAsync";
            using (var operation = telemetry.StartOperation(operationName, $"{Guid.NewGuid():N}"))
            {
                //logger.LogDebug($"Activating process learner actor: {Id}");
                
                await base.OnActivateAsync().ConfigureAwait(false);
                //TrackInfrastructureEvent("operationName", stopwatch);
                telemetry.StopOperation(operation);
                //logger.LogInfo($"Finished activating process learner actor: {Id}");
            }
        }
        
        public async Task<List<EarningEvent>> CreateLearnerEarningEvents(ProcessLearnerCommand processLearnerCommand)
        {
            var earningEvents = new List<EarningEvent>();

            if (await duplicateLearnerService.IsDuplicate(processLearnerCommand, CancellationToken.None))
            {
                return earningEvents;
            }

            logger.LogDebug($"Handling ILR learner submission. Job: {processLearnerCommand.JobId}, Collection year: {processLearnerCommand.CollectionYear}, Learner: {processLearnerCommand.Learner.LearnRefNumber}");
            var processorResult = learnerSubmissionProcessor.GenerateEarnings(processLearnerCommand);
            if (processorResult.Validation.Failed)
            {
                logger.LogInfo($"ILR Learner Submission failed validation. Job: {processLearnerCommand.JobId}, Collection year: {processLearnerCommand.CollectionYear}, Learner: {processLearnerCommand.Learner.LearnRefNumber}");
                return earningEvents;
            }

            earningEvents = processorResult.EarningEvents;
            return earningEvents;
        }
        
        //private void TrackInfrastructureEvent(string eventName, Stopwatch stopwatch)
        //{
        //    telemetry.TrackEvent(eventName,
        //        new Dictionary<string, string>
        //        {
        //            { "ActorId", Id.ToString()},
        //            { TelemetryKeys.Ukprn, Id.ToString()},
        //        },
        //        new Dictionary<string, double>
        //        {
        //            { TelemetryKeys.Duration, stopwatch.ElapsedMilliseconds }
        //        });
        //}
    }
}
