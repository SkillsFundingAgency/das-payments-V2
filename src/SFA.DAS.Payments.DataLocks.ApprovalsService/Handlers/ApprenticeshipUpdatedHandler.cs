using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Handlers
{
    public class ApprenticeshipUpdatedHandler : BaseApprovalsMessageHandler<ApprenticeshipUpdatedApprovedEvent>
    {
        public ApprenticeshipUpdatedHandler(IPaymentLogger logger, IContainerScopeFactory factory) : base(logger, factory)
        {
        }

        protected override async Task HandleMessage(ApprenticeshipUpdatedApprovedEvent message, IMessageHandlerContext context, ILifetimeScope scope)
        {
            Logger.LogDebug($"Handling apprenticeship updated event.  " +
                            $"Now resolving the apprenticeship processor service to handle Updated apprenticeship. " +
                            $"Apprenticeship Id: {message.ApprenticeshipId}");

            var processor = scope.Resolve<IApprenticeshipProcessor>();

          await processor.ProcessUpdatedApprenticeship(message);

            Logger.LogInfo($"Finished handling apprenticeship created event.  " +
                           $"Now resolving the apprenticeship processor service to handle the new apprenticeship. " +
                           $"Apprenticeship Id: {message.ApprenticeshipId}");
        }
    }
}