using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData
{
    public interface IApprenticeshipsDataService
    {
        Task ProcessComparison();
    }

    public class ApprenticeshipDataService : IApprenticeshipsDataService
    {
        private const string DasApproved = "DasApproved";
        private const string DasStopped = "DasStopped";
        private const string DasPaused = "DasPaused";
        private const string PaymentsApproved = "PaymentsApproved";
        private const string PaymentsStopped = "PaymentsStopped";
        private const string PaymentsPaused = "PaymentsPaused";
        private const string ApprovalsReferenceDataComparisonEvent = "ApprovalsReferenceDataComparisonEvent";

        private readonly IPaymentsDataContextFactory paymentsDataContextFactory;
        private readonly ICommitmentsApiClient commitmentsApiClient;
        private readonly ITelemetry telemetry;

        private IPaymentsDataContext PaymentsDataContext => paymentsDataContextFactory.Create();

        public ApprenticeshipDataService(IPaymentsDataContextFactory paymentsDataContextFactory, ICommitmentsApiClient commitmentsApiClient, ITelemetry telemetry)
        {
            this.paymentsDataContextFactory = paymentsDataContextFactory;
            this.commitmentsApiClient = commitmentsApiClient;
            this.telemetry = telemetry;
        }

        public async Task ProcessComparison()
        {
            var pastThirtyDays = DateTime.UtcNow.AddDays(-30).Date;

            var commitmentsTask = commitmentsApiClient.GetStats(30);

            var paymentsApprovedTask = PaymentsDataContext.Apprenticeship
                .CountAsync(paymentsApprenticeship => paymentsApprenticeship.CreationDate > pastThirtyDays);

            var paymentsStoppedTask = PaymentsDataContext.Apprenticeship
                .CountAsync(paymentsApprenticeship =>
                    paymentsApprenticeship.Status == ApprenticeshipStatus.Stopped
                    && paymentsApprenticeship.StopDate > pastThirtyDays);

            var paymentsPausedTask = PaymentsDataContext.Apprenticeship.Include(x => x.ApprenticeshipPauses)
                .CountAsync(paymentsApprenticeship =>
                    paymentsApprenticeship.Status == ApprenticeshipStatus.Paused
                    && paymentsApprenticeship.ApprenticeshipPauses.Any(pause =>
                        pause.PauseDate > pastThirtyDays
                        && pause.ResumeDate == null));

            await Task.WhenAll(commitmentsTask, paymentsApprovedTask, paymentsStoppedTask, paymentsPausedTask).ConfigureAwait(false);

            var commitmentsCount = commitmentsTask.Result;

            var paymentsApprovedCount = paymentsApprovedTask.Result;
            var paymentsStoppedCount = paymentsStoppedTask.Result;
            var paymentsPausedCount = paymentsPausedTask.Result;

            telemetry.TrackEvent(ApprovalsReferenceDataComparisonEvent, new Dictionary<string, double>
            {
                { DasApproved, commitmentsCount.Approved },
                { DasStopped, commitmentsCount.Stopped },
                { DasPaused, commitmentsCount.Paused },
                { PaymentsApproved, paymentsApprovedCount },
                { PaymentsStopped, paymentsStoppedCount },
                { PaymentsPaused, paymentsPausedCount },
            });
        }
    }
}
