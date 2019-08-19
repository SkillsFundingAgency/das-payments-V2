using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Domain.Models;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class HandleIlrSubmissionService : IHandleIlrSubmissionService
    {
        private readonly IProviderPaymentsRepository providerPaymentsRepository;
        private readonly IDataCache<ReceivedProviderEarningsEvent> ilrSubmittedEventCache;
        private readonly IValidateIlrSubmission validateIlrSubmission;
        private readonly IPaymentLogger logger;

        public HandleIlrSubmissionService(IProviderPaymentsRepository providerPaymentsRepository,
            IDataCache<ReceivedProviderEarningsEvent> ilrSubmittedEventCache,
            IValidateIlrSubmission validateIlrSubmission,
            IPaymentLogger logger)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
            this.ilrSubmittedEventCache = ilrSubmittedEventCache;
            this.validateIlrSubmission = validateIlrSubmission;
            this.logger = logger;
        }

        public async Task Handle(ReceivedProviderEarningsEvent message, CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Handling ILR Submissions. Data: {message.ToJson()}");
            var currentIlr = await GetCurrentIlrSubmissionEvent(message.Ukprn, cancellationToken);
            var isNewIlrSubmission = validateIlrSubmission.IsNewIlrSubmission(new IlrSubmissionValidationRequest
            {
                IncomingPaymentUkprn = message.Ukprn,
                IncomingPaymentSubmissionDate = message.IlrSubmissionDateTime,
                CurrentIlr = currentIlr
            });

            if (!isNewIlrSubmission)
            {
                logger.LogInfo($"Ignored same Ilr Submission Data for Ukprn {message.Ukprn} and Job Id {message.JobId} Submission already processed");
                return;
            }

            logger.LogInfo($"Updating current Ilr Submission Data for Ukprn {message.Ukprn} and Job Id {message.JobId}");

            await ilrSubmittedEventCache.Clear(message.Ukprn.ToString(), cancellationToken);
            await ilrSubmittedEventCache.Add(message.Ukprn.ToString(), message, cancellationToken);

            logger.LogDebug($"Successfully Updated current Ilr Submission Data for Ukprn {message.Ukprn} and Job Id {message.JobId}");

            await providerPaymentsRepository.DeleteOldMonthEndPayment(message.CollectionPeriod,
                message.Ukprn,
                message.IlrSubmissionDateTime,
                cancellationToken);

            logger.LogInfo($"Successfully Deleted Old Month End Payment for Ukprn {message.Ukprn} and Job Id {message.JobId}");
        }

        public async Task HandleSubmissionSucceeded(short academicYear, byte collectionPeriod, long ukprn, DateTime submissionTime, long jobId, CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Handling Submission Succeeded. Data: Ukprn: {ukprn}, Academic Year: {academicYear}, Collection Period: {collectionPeriod}, Submission Time {submissionTime}");
            await providerPaymentsRepository.DeleteOldMonthEndPayment(new CollectionPeriod { AcademicYear = academicYear, Period = collectionPeriod},
                ukprn,
                submissionTime,
                cancellationToken);

            logger.LogInfo($"Successfully Deleted Old Month End Payment for Ukprn: {ukprn}, Academic Year: {academicYear}, Collection Period: {collectionPeriod}, Submission Time {submissionTime} and Job Id {jobId}");
        }

        public async Task HandleSubmissionFailed(short academicYear, byte collectionPeriod, long ukprn, DateTime submissionTime, long jobId, CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Handling Submission Succeeded. Data: Ukprn: {ukprn}, Academic Year: {academicYear}, Collection Period: {collectionPeriod}, Submission Time {submissionTime}");
            await providerPaymentsRepository.DeleteCurrentMonthEndPayment(new CollectionPeriod { AcademicYear = academicYear, Period = collectionPeriod },
                ukprn,
                submissionTime,
                cancellationToken);
            logger.LogInfo($"Successfully Deleted Current Month End Payment for Ukprn: {ukprn}, Academic Year: {academicYear}, Collection Period: {collectionPeriod}, Submission Time {submissionTime} and Job Id {jobId}");
        }

        private async Task<ReceivedProviderEarningsEvent> GetCurrentIlrSubmissionEvent(long ukprn, CancellationToken cancellationToken)
        {
            var currentSubmittedIlrConditionalValue = await ilrSubmittedEventCache.TryGet(ukprn.ToString(), cancellationToken);
            return currentSubmittedIlrConditionalValue.HasValue ? currentSubmittedIlrConditionalValue.Value : null;
        }
    }
}