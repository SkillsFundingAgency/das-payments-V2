using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent
{
    public interface IEarningsDuplicateEliminator
    {
        List<EarningEvents.Messages.Events.EarningEvent> RemoveDuplicates(
            List<EarningEvents.Messages.Events.EarningEvent> earningEvents);
    }

    public class EarningsDuplicateEliminator: IEarningsDuplicateEliminator
    {
        public List<EarningEvents.Messages.Events.EarningEvent> RemoveDuplicates(List<EarningEvents.Messages.Events.EarningEvent> earningEvents)
        {
            return earningEvents
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
        }
    }
}