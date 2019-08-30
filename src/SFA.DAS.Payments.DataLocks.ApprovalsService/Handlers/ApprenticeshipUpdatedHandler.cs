using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Services;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Handlers
{
    public class ApprenticeshipUpdatedHandler : BaseApprovalsMessageHandler<ApprenticeshipUpdatedApprovedEvent>
    {
        public ApprenticeshipUpdatedHandler(IPaymentLogger logger, IContainerScopeFactory factory, IPeriodEndEventRepository periodEndEventRepository, IDeferredApprovalsEventRepository deferredApprovalsEventRepository, IConfigurationHelper configurationHelper) 
            : base(logger, factory, periodEndEventRepository, deferredApprovalsEventRepository, configurationHelper)
        {
        }

        protected override async Task HandleMessage(ApprenticeshipUpdatedApprovedEvent message, IMessageHandlerContext context, ILifetimeScope scope)
        {
            Logger.LogDebug($"Handling apprenticeship updated event.  " +
                            $"Now resolving the apprenticeship processor service to handle Updated apprenticeship. " +
                            $"Apprenticeship Id: {message.ApprenticeshipId}");

            var processor = scope.Resolve<IApprenticeshipProcessor>();

          await processor.ProcessUpdatedApprenticeship(message);

            Logger.LogInfo($"Finished handling apprenticeship updated event.  " +
                           $"Now resolving the apprenticeship processor service to handle the new apprenticeship. " +
                           $"Apprenticeship Id: {message.ApprenticeshipId}");
        }
    }
}