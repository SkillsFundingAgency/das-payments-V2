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
    public class ApprenticeshipCreatedHandler : BaseApprovalsMessageHandler<ApprenticeshipCreatedEvent>
    {
        public ApprenticeshipCreatedHandler(IPaymentLogger logger, IContainerScopeFactory factory, IPeriodEndEventRepository periodEndEventRepository, IDeferredApprovalsEventRepository deferredApprovalsEventRepository, IConfigurationHelper configurationHelper) 
            : base(logger, factory, periodEndEventRepository, deferredApprovalsEventRepository, configurationHelper)
        {
        }

        protected override async Task HandleMessage(ApprenticeshipCreatedEvent message, IMessageHandlerContext context, ILifetimeScope scope)
        {
            Logger.LogDebug($"Handling apprenticeship created event.  Now resolving the apprenticeship processor service to handle the new apprenticeship. Apprenticeship Id: {message.ApprenticeshipId}, Account: {message.AccountId}, Ukprn: {message.ProviderId}");
            var processor = scope.Resolve<IApprenticeshipProcessor>();
            await processor.Process(message);
            Logger.LogInfo($"Finished handling apprenticeship created event.  Now resolving the apprenticeship processor service to handle the new apprenticeship. Apprenticeship Id: {message.ApprenticeshipId}, Account: {message.AccountId}, Ukprn: {message.ProviderId}");
        }
    }
}