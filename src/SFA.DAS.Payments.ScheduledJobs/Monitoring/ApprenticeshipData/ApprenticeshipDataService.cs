using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData
{
    public class ApprenticeshipDataService : IApprenticeshipsDataService
    {
        private const string DasApproved = "DasApproved";
        private const string DasStopped = "DasStopped";
        private const string DasPaused = "DasPaused";
        private const string PaymentsApproved = "PaymentsApproved";
        private const string PaymentsStopped = "PaymentsStopped";
        private const string PaymentsPaused = "PaymentsPaused";
        private const string ApprovalsReferenceDataComparisonEvent = "ApprovalsReferenceDataComparisonEvent";

        private readonly IPaymentsDataContext paymentsDataContext;
        private readonly ICommitmentsDataContext commitmentsDataContext;
        private readonly ITelemetry telemetry;

        public ApprenticeshipDataService(IPaymentsDataContext paymentsDataContext, ICommitmentsDataContext commitmentsDataContext, ITelemetry telemetry)
        {
            this.paymentsDataContext = paymentsDataContext;
            this.commitmentsDataContext = commitmentsDataContext;
            this.telemetry = telemetry;
        }

        public async Task ProcessComparison()
        {
            var pastThirtyDays = DateTime.UtcNow.AddDays(-30).Date;

            var commitmentsApprovedTask = commitmentsDataContext.Apprenticeship
                .CountAsync(commitmentsApprenticeship =>
                    commitmentsApprenticeship.Commitment.EmployerAndProviderApprovedOn > pastThirtyDays
                    && commitmentsApprenticeship.Commitment.Approvals == 3);

            var commitmentsStoppedTask = commitmentsDataContext.Apprenticeship
                .CountAsync(commitmentsApprenticeship =>
                    commitmentsApprenticeship.StopDate > pastThirtyDays
                    && commitmentsApprenticeship.PaymentStatus == PaymentStatus.Withdrawn);

            var commitmentsPausedTask = commitmentsDataContext.Apprenticeship
                .CountAsync(commitmentsApprenticeship =>
                    commitmentsApprenticeship.IsApproved
                    && commitmentsApprenticeship.PauseDate > pastThirtyDays
                    && commitmentsApprenticeship.PaymentStatus == PaymentStatus.Paused);

            var paymentsApprovedTask = paymentsDataContext.Apprenticeship
                .CountAsync(paymentsApprenticeship => paymentsApprenticeship.CreationDate > pastThirtyDays);

            var paymentsStoppedTask = paymentsDataContext.Apprenticeship
                .CountAsync(paymentsApprenticeship =>
                    paymentsApprenticeship.Status == ApprenticeshipStatus.Stopped
                    && paymentsApprenticeship.StopDate > pastThirtyDays);

            var paymentsPausedTask = paymentsDataContext.Apprenticeship
                .CountAsync(paymentsApprenticeship =>
                    paymentsApprenticeship.Status == ApprenticeshipStatus.Paused
                    && paymentsApprenticeship.ApprenticeshipPause.PauseDate > pastThirtyDays
                    && paymentsApprenticeship.ApprenticeshipPause.ResumeDate == null);

            await Task.WhenAll(commitmentsApprovedTask, commitmentsStoppedTask, commitmentsPausedTask, paymentsApprovedTask, paymentsStoppedTask, paymentsPausedTask).ConfigureAwait(false);

            var commitmentsApprovedCount = commitmentsApprovedTask.Result;
            var commitmentsStoppedCount = commitmentsStoppedTask.Result;
            var commitmentsPausedCount = commitmentsPausedTask.Result;

            var paymentsApprovedCount = paymentsApprovedTask.Result;
            var paymentsStoppedCount = paymentsStoppedTask.Result;
            var paymentsPausedCount = paymentsPausedTask.Result;

            telemetry.TrackEvent(ApprovalsReferenceDataComparisonEvent, new Dictionary<string, double>
            {
                { DasApproved, commitmentsApprovedCount },
                { DasStopped, commitmentsStoppedCount },
                { DasPaused, commitmentsPausedCount },
                { PaymentsApproved, paymentsApprovedCount },
                { PaymentsStopped, paymentsStoppedCount },
                { PaymentsPaused, paymentsPausedCount },
            });
        }
    }
}
