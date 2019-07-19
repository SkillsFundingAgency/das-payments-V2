using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.EarningEvents.SubmissionEventsService.Handlers
{
    public class EarningEventHandler : IHandleMessages<IContractTypeEarningEvent>
    {
        private readonly ISubmissionEventGeneratorService submissionEventGeneratorService;

        public EarningEventHandler(ISubmissionEventGeneratorService submissionEventGeneratorService)
        {
            this.submissionEventGeneratorService = submissionEventGeneratorService;
        }

        public async Task Handle(IContractTypeEarningEvent message, IMessageHandlerContext context)
        {
            await submissionEventGeneratorService.ProcessEarningEvent(message, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
