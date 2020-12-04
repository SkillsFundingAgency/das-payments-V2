using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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

        private readonly IPaymentsDataContextFactory paymentsDataContextFactory;
        private readonly ICommitmentsDataContextFactory commitmentsDataContextFactory;
        private readonly ITelemetry telemetry;

        private IPaymentsDataContext PaymentsDataContext => paymentsDataContextFactory.Create();
        private ICommitmentsDataContext CommitmentsDataContext => commitmentsDataContextFactory.Create();

        public ApprenticeshipDataService(IPaymentsDataContextFactory paymentsDataContextFactory, ICommitmentsDataContextFactory commitmentsDataContextFactory, ITelemetry telemetry)
        {
            this.paymentsDataContextFactory = paymentsDataContextFactory;
            this.commitmentsDataContextFactory = commitmentsDataContextFactory;
            this.telemetry = telemetry;
        }

        public async Task ProcessComparison()
        {
            var pastThirtyDays = DateTime.UtcNow.AddDays(-30).Date;

            var commitmentsApprovedTask = CommitmentsDataContext.Apprenticeship.Include(x => x.Commitment)
                .CountAsync(commitmentsApprenticeship =>
                    commitmentsApprenticeship.Commitment.EmployerAndProviderApprovedOn > pastThirtyDays
                    && commitmentsApprenticeship.Commitment.Approvals == 3);

            var commitmentsStoppedTask = CommitmentsDataContext.Apprenticeship
                .CountAsync(commitmentsApprenticeship =>
                    commitmentsApprenticeship.StopDate > pastThirtyDays
                    && commitmentsApprenticeship.PaymentStatus == PaymentStatus.Withdrawn);

            var commitmentsPausedTask = CommitmentsDataContext.Apprenticeship
                .CountAsync(commitmentsApprenticeship =>
                    commitmentsApprenticeship.IsApproved
                    && commitmentsApprenticeship.PauseDate > pastThirtyDays
                    && commitmentsApprenticeship.PaymentStatus == PaymentStatus.Paused);

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
