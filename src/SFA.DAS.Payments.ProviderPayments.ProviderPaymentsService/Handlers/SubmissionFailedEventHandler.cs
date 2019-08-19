using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class SubmissionFailedEventHandler : SubmissionEventHandler, IHandleMessages<SubmissionFailedEvent>
    {
        public SubmissionFailedEventHandler(IPaymentLogger paymentLogger,
            IHandleIlrSubmissionService submissionService) : base(paymentLogger, submissionService)
        {
        }

        public async Task Handle(SubmissionFailedEvent message, IMessageHandlerContext context)
        {
            await base.Handle(message, context);
        }

        protected override async Task HandleSubmission(IHandleIlrSubmissionService service, SubmissionEvent message)
        {
            await service.HandleSubmissionFailed(message.AcademicYear, message.CollectionPeriod, message.Ukprn, message.IlrSubmissionDateTime, message.JobId, default(CancellationToken));
        }
    }
}