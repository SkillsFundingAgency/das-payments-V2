using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class SubmissionFailedEventHandler : SubmissionEventHandler<SubmissionJobFailed>
    {
        public SubmissionFailedEventHandler(IPaymentLogger paymentLogger,
            IHandleIlrSubmissionService submissionService) : base(paymentLogger, submissionService)
        {
        }

        protected override async Task HandleSubmission(SubmissionJobFailed message, IHandleIlrSubmissionService service)
        {
            await service.HandleSubmissionFailed(message.AcademicYear, message.CollectionPeriod, message.Ukprn, message.IlrSubmissionDateTime, message.JobId, default(CancellationToken));
        }
    }
}