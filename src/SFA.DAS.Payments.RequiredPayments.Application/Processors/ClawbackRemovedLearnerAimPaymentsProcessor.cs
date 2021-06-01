using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper mapper;
        private readonly IPaymentLogger logger;

        public ClawbackRemovedLearnerAimPaymentsProcessor(IPaymentHistoryRepository paymentHistoryRepository, IMapper mapper, IPaymentLogger logger)
        {
            this.paymentHistoryRepository = paymentHistoryRepository ?? throw new ArgumentNullException(nameof(paymentHistoryRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

            if (!learnerPaymentHistory.Any() || learnerPaymentHistory.Sum(p => p.Amount) == 0)
            {
                return new List<CalculatedRequiredLevyAmount>();
            }

            if (learnerPaymentHistory.All(p => p.ClawbackSourcePaymentId == null))
            {
                learnerPaymentHistory.ForEach(p =>
                {
                    p.Amount *= -1;
                    p.JobId = message.JobId;
                    p.ClawbackSourcePaymentId = p.EventId;
                    p.CollectionPeriod = message.CollectionPeriod.Clone();
                    p.IlrSubmissionDateTime = message.IlrSubmissionDateTime;
                });

                return mapper.Map<List<CalculatedRequiredLevyAmount>>(learnerPaymentHistory);
            }

            return new List<CalculatedRequiredLevyAmount>();
        }
    }
}