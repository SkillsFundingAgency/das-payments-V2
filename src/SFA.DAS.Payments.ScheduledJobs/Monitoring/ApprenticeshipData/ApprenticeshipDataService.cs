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
                .Where(a => a.Commitment.EmployerAndProviderApprovedOn >= pastThirtyDays)
                .CountAsync(x => x.IsApproved);

            var commitmentsStatusTask = commitmentsDataContext.Apprenticeship
                .Where(a => a.PaymentStatus == PaymentStatus.Withdrawn || a.PaymentStatus == PaymentStatus.Paused)
                .Where(a => a.Commitment.EmployerAndProviderApprovedOn >= pastThirtyDays)
                .GroupBy(g => g.PaymentStatus)
                .Select(x => new { x.Key, Count = x.Count() })
                .ToListAsync();

            var paymentsStatusTask = paymentsDataContext.Apprenticeship
                .Where(a => a.Status == ApprenticeshipStatus.Active || a.Status == ApprenticeshipStatus.Stopped || a.Status == ApprenticeshipStatus.Paused)
                .Where(a => a.CreationDate >= pastThirtyDays)
                .GroupBy(g => g.Status)
                .Select(x => new { x.Key, Count = x.Count() })
                .ToListAsync();

            await Task.WhenAll(commitmentsApprovedTask, commitmentsStatusTask, paymentsStatusTask).ConfigureAwait(false);

            var commitmentsApprovedCount = commitmentsApprovedTask.Result;
            var commitmentsStoppedCount = (commitmentsStatusTask.Result.SingleOrDefault(x => x.Key == PaymentStatus.Withdrawn)?.Count).GetValueOrDefault();
            var commitmentsPausedCount = (commitmentsStatusTask.Result.SingleOrDefault(x => x.Key == PaymentStatus.Paused)?.Count).GetValueOrDefault();

            var paymentsApprovedCount = (paymentsStatusTask.Result.SingleOrDefault(x => x.Key == ApprenticeshipStatus.Active)?.Count).GetValueOrDefault();
            var paymentsStoppedCount = (paymentsStatusTask.Result.SingleOrDefault(x => x.Key == ApprenticeshipStatus.Stopped)?.Count).GetValueOrDefault();
            var paymentsPausedCount = (paymentsStatusTask.Result.SingleOrDefault(x => x.Key == ApprenticeshipStatus.Paused)?.Count).GetValueOrDefault();

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
