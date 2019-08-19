using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class SubmissionSucceededEventHandler: SubmissionEventHandler<SubmissionSucceededEvent>
    {
        public SubmissionSucceededEventHandler(IPaymentLogger paymentLogger,
            IHandleIlrSubmissionService submissionService): base(paymentLogger, submissionService)
        {
        }
       
        protected override async Task HandleSubmission(SubmissionSucceededEvent message, IHandleIlrSubmissionService service)
        {
            await service.HandleSubmissionSucceeded(message.AcademicYear, message.CollectionPeriod, message.Ukprn, message.IlrSubmissionDateTime, message.JobId, default(CancellationToken));
        }
    }
}