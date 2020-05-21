using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Domain;
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

        public ProcessLearnerService(
            ActorService actorService,
            ActorId actorId,
            ILearnerSubmissionProcessor learnerSubmissionProcessor, 
            IPaymentLogger logger
            ) : base(actorService, actorId)
        {
            this.learnerSubmissionProcessor = learnerSubmissionProcessor ?? throw new ArgumentNullException(nameof(learnerSubmissionProcessor));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync("count", 0);
        }


        public List<EarningEvent> CreateLearnerEarningEvents(ProcessLearnerCommand processLearnerCommand)
        {
            logger.LogDebug($"Handling ILR learner submission. Job: {processLearnerCommand.JobId}, Ukprn: {processLearnerCommand.Ukprn}, Collection year: {processLearnerCommand.CollectionYear}, Learner: {processLearnerCommand.Learner.LearnRefNumber}");
            var processorResult = learnerSubmissionProcessor.GenerateEarnings(processLearnerCommand);
            if (processorResult.Validation.Failed)
            {
                logger.LogInfo($"ILR Learner Submission failed validation. Job: {processLearnerCommand.JobId}, Ukprn: {processLearnerCommand.Ukprn}, Collection year: {processLearnerCommand.CollectionYear}, Learner: {processLearnerCommand.Learner.LearnRefNumber}");
                return new List<EarningEvent>();
            }
            return processorResult.EarningEvents;
        }
    }
}
