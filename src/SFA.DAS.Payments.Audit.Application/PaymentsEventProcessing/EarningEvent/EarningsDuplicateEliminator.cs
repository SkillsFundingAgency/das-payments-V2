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
            var alreadyStored = await repository.GetAlreadyStoredEarnings(models, cancellationToken).ConfigureAwait(false);
            models.RemoveAll(model => alreadyStored.Any(storedEarning => 
                        storedEarning.Ukprn == model.Ukprn && 
                        storedEarning.ContractType == model.ContractType &&
                        storedEarning.CollectionPeriod == model.CollectionPeriod &&
                        storedEarning.AcademicYear == model.AcademicYear &&
                        storedEarning.LearnerReferenceNumber == model.LearnerReferenceNumber &&
                        storedEarning.LearnerUln == model.LearnerUln &&
                        storedEarning.LearningAimReference == model.LearningAimReference &&
                        storedEarning.LearningAimProgrammeType == model.LearningAimProgrammeType &&
                        storedEarning.LearningAimStandardCode == model.LearningAimStandardCode &&
                        storedEarning.LearningAimFrameworkCode == model.LearningAimFrameworkCode &&
                        storedEarning.LearningAimPathwayCode == model.LearningAimPathwayCode &&
                        storedEarning.LearningAimFundingLineType == model.LearningAimFundingLineType &&
                        storedEarning.LearningStartDate == model.LearningStartDate &&
                        storedEarning.JobId == model.JobId &&
                        storedEarning.LearningAimSequenceNumber == model.LearningAimSequenceNumber &&
                        storedEarning.EventType == model.EventType));
            return models;
        }
    }
}