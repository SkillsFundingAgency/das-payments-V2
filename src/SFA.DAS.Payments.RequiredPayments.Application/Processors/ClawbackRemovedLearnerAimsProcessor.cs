using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public interface IClawbackRemovedLearnerAimsProcessor
    {
        Task<IList<PeriodisedRequiredPaymentEvent>> GenerateClawbackForRemovedLearnerAim(IdentifiedRemovedLearningAim message, CancellationToken cancellationToken);
    }

    public class ClawbackRemovedLearnerAimsProcessor : IClawbackRemovedLearnerAimsProcessor
    {
        // ReSharper disable IdentifierTypo
        public readonly IRequiredPaymentEventFactory requiredPaymentEventFactory;
        private readonly IPaymentClawbackRepository paymentClawbackRepository;
        private readonly IMapper mapper;
        private readonly IPaymentLogger logger;

        public ClawbackRemovedLearnerAimsProcessor(IRequiredPaymentEventFactory requiredPaymentEventFactory, IPaymentClawbackRepository paymentClawbackRepository, IMapper mapper, IPaymentLogger logger)
        {
            this.requiredPaymentEventFactory = requiredPaymentEventFactory ?? throw new ArgumentNullException(nameof(requiredPaymentEventFactory));
            this.paymentClawbackRepository = paymentClawbackRepository ?? throw new ArgumentNullException(nameof(paymentClawbackRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IList<PeriodisedRequiredPaymentEvent>> GenerateClawbackForRemovedLearnerAim(IdentifiedRemovedLearningAim message, CancellationToken cancellationToken)
        {
            var learnerPaymentHistory = await paymentClawbackRepository.GetReadOnlyLearnerPaymentHistory(
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
                logger.LogInfo("no previous payments or sum of all previous payments is already zero so no action required" +
                               $"jobId:{message.JobId}, learnerRef:{message.Learner.ReferenceNumber}, frameworkCode:{message.LearningAim.FrameworkCode}, " +
                               $"pathwayCode:{message.LearningAim.PathwayCode}, programmeType:{message.LearningAim.ProgrammeType}, " +
                               $"standardCode:{message.LearningAim.StandardCode}, learningAimReference:{message.LearningAim.Reference}, " +
                               $"academicYear:{message.CollectionPeriod.AcademicYear}, contractType:{message.ContractType}");

                return new List<PeriodisedRequiredPaymentEvent>();
            }

            var paymentToIgnore = learnerPaymentHistory.Join(learnerPaymentHistory, 
                    payment => payment.EventId, 
                    clawbackPayment => clawbackPayment.ClawbackSourcePaymentEventId,
                    (payment, clawbackPayment) => new[] {payment.EventId, clawbackPayment.EventId})
                .SelectMany(paymentId => paymentId);

            if (learnerPaymentHistory.Where(payment => paymentToIgnore.Contains(payment.EventId)).Sum(p => p.Amount) != 0)
            {
                logger.LogWarning("Previous Payment and Clawback do not Match, this clawback will result in over or under payment" +
                               $"jobId:{message.JobId}, learnerRef:{message.Learner.ReferenceNumber}, frameworkCode:{message.LearningAim.FrameworkCode}, " +
                               $"pathwayCode:{message.LearningAim.PathwayCode}, programmeType:{message.LearningAim.ProgrammeType}, " +
                               $"standardCode:{message.LearningAim.StandardCode}, learningAimReference:{message.LearningAim.Reference}, " +
                               $"academicYear:{message.CollectionPeriod.AcademicYear}, contractType:{message.ContractType}");

            }

            var paymentToClawback = learnerPaymentHistory
                .Where(payment => !paymentToIgnore.Contains(payment.EventId))
                .Select(payment =>
                {
                    ConvertToClawbackPayment(message, payment);
                    return payment;
                }).ToList();

            return ConvertToRequiredPaymentEvent(paymentToClawback);
        }

        private static void ConvertToClawbackPayment(IdentifiedRemovedLearningAim message, PaymentModel clawbackPayment)
        {
            clawbackPayment.Id = 0;
            clawbackPayment.Amount *= -1;
            clawbackPayment.JobId = message.JobId;
            clawbackPayment.CollectionPeriod = message.CollectionPeriod.Clone();
            clawbackPayment.IlrSubmissionDateTime = message.IlrSubmissionDateTime;
            clawbackPayment.EventTime = DateTimeOffset.UtcNow;

            clawbackPayment.RequiredPaymentEventId = Guid.Empty;
            clawbackPayment.EarningEventId = Guid.Empty;
            clawbackPayment.FundingSourceEventId = Guid.Empty;

            //NOTE: DO NOT CHANGE THE ORDER OF ASSIGNMENT BELLOW
            clawbackPayment.ClawbackSourcePaymentEventId = clawbackPayment.EventId;
            clawbackPayment.EventId = Guid.NewGuid();
        }

        private List<PeriodisedRequiredPaymentEvent> ConvertToRequiredPaymentEvent(List<PaymentModel> paymentToClawback)
        {
            return paymentToClawback
                .GroupBy(x => new
                {
                    x.SfaContributionPercentage,
                    EarningType = ToEarningType(x.FundingSource),
                    x.PriceEpisodeIdentifier,
                    x.AccountId,
                    x.TransferSenderAccountId,
                    x.ApprenticeshipEmployerType,
                    x.ApprenticeshipId,
                    x.ApprenticeshipPriceEpisodeId,
                    x.LearningStartDate,
                    x.DeliveryPeriod,
                    x.Ukprn,
                    x.ContractType,
                })
                .Select(group =>
                {
                    var payment = group.First();
                    
                    var amountForGroup = group.Sum(x => x.Amount);
                    
                    var requiredPayment = requiredPaymentEventFactory.Create(group.Key.EarningType, payment.TransactionType, group.Key.SfaContributionPercentage, amountForGroup.AsRounded(),);

                    mapper.Map(payment, requiredPayment);

                    return requiredPayment;
                })
                .ToList();
        }

        private static EarningType ToEarningType(FundingSourceType fundingSource)
        {
            switch (fundingSource)
            {
                case FundingSourceType.Transfer:
                case FundingSourceType.Levy:
                    return EarningType.Levy;


                case FundingSourceType.CoInvestedEmployer:
                case FundingSourceType.CoInvestedSfa:
                    return EarningType.CoInvested;

                case FundingSourceType.FullyFundedSfa:
                    return EarningType.Incentive;
                default:
                    throw new NotImplementedException($"Unknown funding source: {fundingSource}");
            }
        }
    }
}