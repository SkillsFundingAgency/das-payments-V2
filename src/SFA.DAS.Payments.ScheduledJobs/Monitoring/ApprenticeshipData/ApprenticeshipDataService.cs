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
        private readonly ITelemetry telemetry;

        private IPaymentsDataContext PaymentsDataContext => paymentsDataContextFactory.Create();

        {
            this.paymentsDataContextFactory = paymentsDataContextFactory;
            this.telemetry = telemetry;
        }

        public async Task ProcessComparison()
        {
            var pastThirtyDays = DateTime.UtcNow.AddDays(-30).Date;


            var paymentsApprovedTask = PaymentsDataContext.Apprenticeship
                .CountAsync(paymentsApprenticeship => paymentsApprenticeship.CreationDate > pastThirtyDays);

            var paymentsStoppedTask = PaymentsDataContext.Apprenticeship
                .CountAsync(paymentsApprenticeship =>
                    paymentsApprenticeship.Status == ApprenticeshipStatus.Stopped
                    && paymentsApprenticeship.StopDate > pastThirtyDays);

            var paymentsPausedTask = PaymentsDataContext.Apprenticeship.Include(x => x.ApprenticeshipPauses)
                .CountAsync(paymentsApprenticeship =>
                    paymentsApprenticeship.Status == ApprenticeshipStatus.Paused
                        pause.PauseDate > pastThirtyDays
                        && pause.ResumeDate == null));



            var paymentsApprovedCount = paymentsApprovedTask.Result;
            var paymentsStoppedCount = paymentsStoppedTask.Result;
            var paymentsPausedCount = paymentsPausedTask.Result;

            telemetry.TrackEvent(ApprovalsReferenceDataComparisonEvent, new Dictionary<string, double>
            {
                { PaymentsApproved, paymentsApprovedCount },
                { PaymentsStopped, paymentsStoppedCount },
                { PaymentsPaused, paymentsPausedCount },
            });
        }
    }
}
