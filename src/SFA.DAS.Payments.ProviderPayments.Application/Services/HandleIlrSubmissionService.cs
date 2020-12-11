using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IHandleIlrSubmissionService
    {
        Task HandleSubmissionSucceeded(short academicYear, byte collectionPeriod, long ukprn, DateTime submissionTime, long jobId, CancellationToken cancellationToken);

        Task HandleSubmissionFailed(short academicYear, byte collectionPeriod, long ukprn, DateTime submissionTime, long jobId, CancellationToken cancellationToken);
    }

    public class HandleIlrSubmissionService : IHandleIlrSubmissionService
    {
        private readonly IProviderPaymentsRepository providerPaymentsRepository;
        private readonly IPaymentLogger logger;

        public HandleIlrSubmissionService(IProviderPaymentsRepository providerPaymentsRepository, IPaymentLogger logger)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
            this.logger = logger;
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
    }
}