﻿using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using System.Threading.Tasks;
using SFA.DAS.Payments.Monitoring.Metrics.Data;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Submission
{
    public interface ISubmissionMetricsService
    {

    }

    public class SubmissionMetricsService : ISubmissionMetricsService
    {
        private readonly IPaymentLogger logger;
        private readonly ISubmissionSummaryFactory submissionSummaryFactory;
        private readonly IDcMetricsDataContext dcDataContext;
        private readonly ISubmissionMetricsRepository submissionRepository;

        public SubmissionMetricsService(IPaymentLogger logger, ISubmissionSummaryFactory submissionSummaryFactory, IDcMetricsDataContext dcDataContext, ISubmissionMetricsRepository submissionRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.submissionSummaryFactory = submissionSummaryFactory ?? throw new ArgumentNullException(nameof(submissionSummaryFactory));
            this.dcDataContext = dcDataContext ?? throw new ArgumentNullException(nameof(dcDataContext));
            this.submissionRepository = submissionRepository ?? throw new ArgumentNullException(nameof(submissionRepository));
        }

        public async Task BuildMetrics(long ukprn, long jobId, short academicYear, byte collectionPeriod)
        {
            var submissionSummary = submissionSummaryFactory.Create(ukprn, jobId, academicYear, collectionPeriod);
            var dcEarningsTask = dcDataContext.GetEarnings(ukprn, academicYear, collectionPeriod);
            var dasEarningsTask = submissionRepository.GetDasEarnings(ukprn, jobId);
            await Task.WhenAll(dasEarningsTask, dcEarningsTask).ConfigureAwait(false);
            submissionSummary.AddEarnings(dcEarningsTask.Result, dasEarningsTask.Result);

        }
    }
}