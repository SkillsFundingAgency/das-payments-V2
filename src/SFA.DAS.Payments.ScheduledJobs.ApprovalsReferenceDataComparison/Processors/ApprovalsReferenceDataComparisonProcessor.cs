using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ScheduledJobs.ApprovalsReferenceDataComparison.Repositories;
using SFA.DAS.Payments.ScheduledJobs.ApprovalsReferenceDataComparison.Repositories.Models;

namespace SFA.DAS.Payments.ScheduledJobs.ApprovalsReferenceDataComparison.Processors
{
    public class ApprovalsReferenceDataComparisonProcessor : IApprovalsReferenceDataComparisonProcessor
    {
        private const string DasApproved = "DasApproved";
        private const string DasStopped = "DasStopped";
        private const string DasPaused = "DasPaused";
        private const string PaymentsApproved = "PaymentsApproved";
        private const string PaymentsStopped = "PaymentsStopped";
        private const string PaymentsPaused = "PaymentsPaused";

        private readonly IPaymentsDataContext paymentsDataContext;
        private readonly ICommitmentsDataContext commitmentsDataContext;
        private readonly ITelemetry telemetry;

        public ApprovalsReferenceDataComparisonProcessor(IPaymentsDataContext paymentsDataContext, ICommitmentsDataContext commitmentsDataContext, ITelemetry telemetry)
        {
            this.paymentsDataContext = paymentsDataContext;
            this.commitmentsDataContext = commitmentsDataContext;
            this.telemetry = telemetry;
        }

        public void ProcessComparison()
        {
            var commitmentsApprovedCount = commitmentsDataContext.Apprenticeships.Count(x => x.IsApproved);
            var commitmentsStoppedCount = commitmentsDataContext.Apprenticeships.Count(x => x.PaymentStatus == PaymentStatus.Withdrawn); //todo is this the right criteria?
            var commitmentsPausedCount = commitmentsDataContext.Apprenticeships.Count(x => x.PaymentStatus == PaymentStatus.Paused);

            var paymentsApprovedCount = paymentsDataContext.Apprenticeship.Count(x => x.Status == ApprenticeshipStatus.Active); //todo check criteria
            var paymentsStoppedCount = paymentsDataContext.Apprenticeship.Count(x => x.Status == ApprenticeshipStatus.Stopped);
            var paymentsPausedCount = paymentsDataContext.Apprenticeship.Count(x => x.Status == ApprenticeshipStatus.Paused);

            telemetry.TrackEvent("ApprovalsReferenceDataComparisonEvent", new Dictionary<string, double>
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
