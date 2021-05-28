using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public interface IClawbackRemovedLearnerAimPaymentsProcessor
    {
        Task<IReadOnlyCollection<CalculatedRequiredLevyAmount>> GenerateClawbackForRemovedLearnerAim(IdentifiedRemovedLearningAim message, CancellationToken cancellationToken);
    }

    public class ClawbackRemovedLearnerAimPaymentsProcessor : IClawbackRemovedLearnerAimPaymentsProcessor
    {
        private readonly IApprenticeshipKeyService apprenticeshipKeyService;
        private readonly IPaymentHistoryRepository paymentHistoryRepository;

        public ClawbackRemovedLearnerAimPaymentsProcessor(IApprenticeshipKeyService apprenticeshipKeyService, IPaymentHistoryRepository paymentHistoryRepository)
        {
            this.apprenticeshipKeyService = apprenticeshipKeyService ?? throw new ArgumentNullException(nameof(apprenticeshipKeyService));
            this.paymentHistoryRepository = paymentHistoryRepository ?? throw new ArgumentNullException(nameof(paymentHistoryRepository));
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

            throw new System.NotImplementedException();
        }
    }
}