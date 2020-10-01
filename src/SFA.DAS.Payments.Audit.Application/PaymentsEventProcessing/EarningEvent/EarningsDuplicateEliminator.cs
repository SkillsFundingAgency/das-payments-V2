using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Data.EarningEvent;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent
{
    public interface IEarningsDuplicateEliminator
    {
        List<EarningEvents.Messages.Events.EarningEvent> RemoveDuplicates(
            List<EarningEvents.Messages.Events.EarningEvent> earningEvents);
    }

    public class EarningsDuplicateEliminator: IEarningsDuplicateEliminator
    {
        private readonly IPaymentLogger logger;

        public EarningsDuplicateEliminator(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public List<EarningEvents.Messages.Events.EarningEvent> RemoveDuplicates(List<EarningEvents.Messages.Events.EarningEvent> earningEvents)
        {
            logger.LogDebug($"Removing duplicates from batch. Batch size: {earningEvents.Count}");
            var deDuplicatedEvents = earningEvents
                .GroupBy(earningEvent => new
                {
                    earningEvent.Ukprn,
                    earningEvent.GetType().FullName,
                    earningEvent.CollectionPeriod.Period,
                    earningEvent.CollectionPeriod.AcademicYear,
                    earningEvent.Learner.ReferenceNumber,
                    earningEvent.Learner.Uln,
                    earningEvent.LearningAim.Reference,
                    earningEvent.LearningAim.ProgrammeType,
                    earningEvent.LearningAim.StandardCode,
                    earningEvent.LearningAim.FrameworkCode,
                    earningEvent.LearningAim.PathwayCode,
                    earningEvent.LearningAim.FundingLineType,
                    earningEvent.LearningAim.StartDate,
                    earningEvent.JobId,
                    earningEvent.LearningAim.SequenceNumber,
                })
                .Select(group => group.FirstOrDefault())
                .Where(earningEvent => earningEvent != null)
                .ToList();
            if (deDuplicatedEvents.Count != earningEvents.Count)
                logger.LogInfo($"Removed '{earningEvents.Count - deDuplicatedEvents.Count}' duplicates from the batch.");
            else
                logger.LogDebug("Found no duplicates in the batch.");
            return deDuplicatedEvents;
        }
    }
}