using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public interface IClawbackRemovedLearnerAimPaymentsProcessor
    {
        Task<IReadOnlyCollection<CalculatedRequiredLevyAmount>> GenerateClawbackForRemovedLearnerAim(IdentifiedRemovedLearningAim message, CancellationToken cancellationToken);
    }

    public class ClawbackRemovedLearnerAimPaymentsProcessor : IClawbackRemovedLearnerAimPaymentsProcessor
    {
        private readonly IPaymentHistoryRepository paymentHistoryRepository;
        private readonly IPaymentLogger logger;

        public ClawbackRemovedLearnerAimPaymentsProcessor(IPaymentHistoryRepository paymentHistoryRepository, IPaymentLogger logger)
        {
            this.paymentHistoryRepository = paymentHistoryRepository ?? throw new ArgumentNullException(nameof(paymentHistoryRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<IReadOnlyCollection<CalculatedRequiredLevyAmount>> GenerateClawbackForRemovedLearnerAim(IdentifiedRemovedLearningAim message, CancellationToken cancellationToken)
        {
            
            var learnerPaymentHistory = await paymentHistoryRepository.GetPaymentHistoryForClawback(
                message.Ukprn,
                message.ContractType,
                message.Learner.ReferenceNumber,
                message.LearningAim.Reference,
                message.LearningAim.FrameworkCode,
                message.LearningAim.PathwayCode,
                message.LearningAim.ProgrammeType, 
                message.LearningAim.StandardCode,
                message.CollectionPeriod.AcademicYear, 
                message.CollectionPeriod.Period, 
                cancellationToken).ConfigureAwait(false);

            return new List<CalculatedRequiredLevyAmount>();
        }
    }
}