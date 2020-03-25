using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Data.EarningEvent;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent
{
    public interface IEarningsDuplicateEliminator
    {
        List<EarningEvents.Messages.Events.EarningEvent> RemoveDuplicates(
            List<EarningEvents.Messages.Events.EarningEvent> earningEvents);

        Task<List<EarningEventModel>> RemoveDuplicates(List<EarningEventModel> models, CancellationToken cancellationToken);
    }

    public class EarningsDuplicateEliminator: IEarningsDuplicateEliminator
    {
        private readonly IEarningEventRepository repository;
        private readonly IPaymentLogger logger;

        public EarningsDuplicateEliminator(IEarningEventRepository repository, IPaymentLogger logger)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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

        public async Task<List<EarningEventModel>> RemoveDuplicates(List<EarningEventModel> models, CancellationToken cancellationToken)
        {
            var alreadyStored = await repository.GetDuplicateEarnings(models, cancellationToken).ConfigureAwait(false);
            models.RemoveAll(model => alreadyStored.Any(stored => stored.EventId == model.EventId));
            return models;
        }
    }
}