using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Services;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Handlers
{
    public class DataLockTriageApprovedEventHandler : BaseApprovalsMessageHandler<DataLockTriageApprovedEvent>
    {
        public DataLockTriageApprovedEventHandler(IPaymentLogger logger, IContainerScopeFactory factory) 
            : base(logger, factory)
        {
        }

        protected override async Task HandleMessage(DataLockTriageApprovedEvent message, IMessageHandlerContext context, ILifetimeScope scope)
        {
            Logger.LogDebug($"Handling apprenticeship DataLock Triage Approved event.  " +
                            $"Now resolving the apprenticeship processor service to handle Updated apprenticeship. " +
                            $"Apprenticeship Id: {message.ApprenticeshipId}");

            var processor = scope.Resolve<IApprenticeshipProcessor>();

         await processor.ProcessApprenticeshipDataLockTriage(message);

            Logger.LogInfo($"Finished handling apprenticeship  DataLock Triage Approved event.  " +
                           $"Now resolving the apprenticeship processor service to handle the new apprenticeship. " +
                           $"Apprenticeship Id: {message.ApprenticeshipId}");
        }
    }
}