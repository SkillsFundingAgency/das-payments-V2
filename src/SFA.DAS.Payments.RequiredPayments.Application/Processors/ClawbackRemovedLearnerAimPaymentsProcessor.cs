using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public interface IClawbackRemovedLearnerAimPaymentsProcessor
    {
        Task<IList<CalculatedRequiredLevyAmount>> GenerateClawbackForRemovedLearnerAim(IdentifiedRemovedLearningAim message, CancellationToken cancellationToken);
    }

    public class ClawbackRemovedLearnerAimPaymentsProcessor : IClawbackRemovedLearnerAimPaymentsProcessor
    {
        private readonly IPaymentClawbackRepository paymentClawbackRepository;
        private readonly IMapper mapper;
        private readonly IPaymentLogger logger;

        public ClawbackRemovedLearnerAimPaymentsProcessor(IPaymentClawbackRepository paymentClawbackRepository, IMapper mapper, IPaymentLogger logger)
        {
            this.paymentClawbackRepository = paymentClawbackRepository ?? throw new ArgumentNullException(nameof(paymentClawbackRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IList<CalculatedRequiredLevyAmount>> GenerateClawbackForRemovedLearnerAim(IdentifiedRemovedLearningAim message, CancellationToken cancellationToken)
        {
            var learnerPaymentHistory = await paymentClawbackRepository.GetLearnerPaymentHistory(
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
                logger.LogDebug("no previous payments or sum of all previous payments is already zero so no action required");

                return new List<CalculatedRequiredLevyAmount>();
            }

            if (learnerPaymentHistory.All(p => p.ClawbackSourcePaymentEventId == null))
            {
                logger.LogDebug("all previous payments needs clawback");

                learnerPaymentHistory.ForEach(p => ConvertToClawbackPayment(message, p));

                return await ProcessPaymentToClawback(learnerPaymentHistory, cancellationToken);
            }

            logger.LogDebug("only some of the previous payments needs clawback");

            var paymentToIgnore = (from payment in learnerPaymentHistory
                                   join clawbackPayment in learnerPaymentHistory
                                       on payment.EventId equals clawbackPayment.ClawbackSourcePaymentEventId
                                   select new[] { payment.EventId, clawbackPayment.EventId })
                .SelectMany(paymentId => paymentId);

            if (learnerPaymentHistory.Where(payment => paymentToIgnore.Contains(payment.EventId)).Sum(p => p.Amount) != 0)
            {
                logger.LogError("Previous Payment and Clawback do not Match, this clawback will result in over or under payment");
            }

            var paymentToClawback = learnerPaymentHistory
                .Where(payment => !paymentToIgnore.Contains(payment.EventId))
                .Select(payment =>
                {
                    ConvertToClawbackPayment(message, payment);
                    return payment;
                }).ToList();

            return await ProcessPaymentToClawback(paymentToClawback, cancellationToken);
        }

        private static void ConvertToClawbackPayment(IdentifiedRemovedLearningAim message, PaymentModel clawbackPayment)
        {
            //NOTE: DO NOTE CHANGE THE ORDER OF ASSIGNMENT BELLOW
            clawbackPayment.Amount *= -1;
            clawbackPayment.JobId = message.JobId;
            clawbackPayment.ClawbackSourcePaymentEventId = clawbackPayment.EventId;
            clawbackPayment.CollectionPeriod = message.CollectionPeriod.Clone();
            clawbackPayment.IlrSubmissionDateTime = message.IlrSubmissionDateTime;
            clawbackPayment.EventId = Guid.NewGuid();
            clawbackPayment.EventTime = DateTimeOffset.UtcNow;

            clawbackPayment.RequiredPaymentEventId = null;
            clawbackPayment.EarningEventId = Guid.Empty;
            clawbackPayment.FundingSourceEventId = Guid.Empty;
        }

        private List<CalculatedRequiredLevyAmount> GetCalculatedRequiredLevyAmountEvents(IEnumerable<PaymentModel> paymentToClawback)
        {
            return mapper.Map<List<CalculatedRequiredLevyAmount>>(paymentToClawback);
        }

        private async Task<List<CalculatedRequiredLevyAmount>> ProcessPaymentToClawback(IList<PaymentModel> paymentToClawback, CancellationToken cancellationToken)
        {
            var sfaPayments = paymentToClawback.Where(p => p.FundingSource != FundingSourceType.Levy && p.FundingSource != FundingSourceType.Transfer);
            await paymentClawbackRepository.SaveClawbackPayments(sfaPayments, cancellationToken);

            var levyPayments = paymentToClawback.Where(p => p.FundingSource == FundingSourceType.Levy || p.FundingSource == FundingSourceType.Transfer);
            return GetCalculatedRequiredLevyAmountEvents(levyPayments);
        }
    }
}