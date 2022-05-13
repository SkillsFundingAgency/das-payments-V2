using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.FlexiPaymentsAdapter.Messages.InboundEvents;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FlexiPaymentsAdapter.FlexiPaymentsAdapterService.Handlers
{
    public class LearningPaymentEventHandler : IHandleMessages<LearningPaymentEvent>
    {
        //private readonly IPeriodEndService periodEndService;

        public LearningPaymentEventHandler()
        {
            //this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //this.periodEndService = periodEndService ?? throw new ArgumentNullException(nameof(periodEndService));
        }

        public async Task Handle(LearningPaymentEvent message, IMessageHandlerContext context)
        {
            var requiredPaymentsEvent = new CalculatedRequiredLevyAmount
            {
                AccountId = message.AccountId,
                CollectionPeriod = message.CollectionPeriod,
                JobId = message.JobId,
                ActualEndDate = message.ActualEndDate,
                AgreedOnDate = message.AgreedOnDate,
                AgreementId = message.AgreementId,
                AmountDue = message.AmountDue,
                ApprenticeshipEmployerType = message.ApprenticeshipEmployerType,
                ApprenticeshipId = message.ApprenticeshipId,
                ApprenticeshipPriceEpisodeId = message.ApprenticeshipPriceEpisodeId,
                ClawbackSourcePaymentEventId = message.ClawbackSourcePaymentEventId,
                CompletionAmount = message.CompletionAmount,
                CompletionStatus = message.CompletionStatus,
                ContractType = message.ContractType,
                DeliveryPeriod = message.DeliveryPeriod,
                EarningEventId = message.EarningEventId,
                EventId = message.EventId,
                EventTime = message.EventTime,
                IlrFileName = message.IlrFileName,
                IlrSubmissionDateTime = message.IlrSubmissionDateTime,
                InstalmentAmount = message.InstalmentAmount,
                Learner = message.Learner,
                LearningAim = message.LearningAim,
                LearningAimSequenceNumber = message.LearningAimSequenceNumber,
                LearningStartDate = message.LearningStartDate,
                NumberOfInstalments = message.NumberOfInstalments,
                OnProgrammeEarningType = message.OnProgrammeEarningType,
                PlannedEndDate = message.PlannedEndDate,
                PriceEpisodeIdentifier = message.PriceEpisodeIdentifier,
                Priority = message.Priority,
                ReportingAimFundingLineType = message.ReportingAimFundingLineType,
                SfaContributionPercentage = message.SfaContributionPercentage,
                StartDate = message.StartDate,
                TransferSenderAccountId = message.TransferSenderAccountId,
                Ukprn = message.Ukprn
            };

            await context.Publish(requiredPaymentsEvent);
        }
    }
}
