using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Services;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Handlers
{
    public class ApprenticeshipPausedEventHandler : BaseApprovalsMessageHandler<ApprenticeshipPausedEvent>
    {
        public ApprenticeshipPausedEventHandler(IPaymentLogger logger, IContainerScopeFactory factory) 
            : base(logger, factory)
        {
        }

        protected override async Task HandleMessage(ApprenticeshipPausedEvent message, IMessageHandlerContext context, ILifetimeScope scope)
        {
            Logger.LogDebug($"Handling apprenticeship Paused event for Apprenticeship Id: {message.ApprenticeshipId}");

            var processor = scope.Resolve<IApprenticeshipProcessor>();
            await processor.ProcessPausedApprenticeship(message);

          Logger.LogInfo($"Finished Handling apprenticeship Paused event for Apprenticeship Id: {message.ApprenticeshipId}");

        }
    }
}