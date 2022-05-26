﻿using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Handlers
{
    public class PaymentOrderChangedEventHandler : BaseApprovalsMessageHandler<PaymentOrderChangedEvent>
    {
        public PaymentOrderChangedEventHandler(ITelemetry telemetry, IPaymentLogger logger, IContainerScopeFactory factory) : base(telemetry, logger, factory)
        {
        }

        protected override async Task HandleMessage(PaymentOrderChangedEvent message, IMessageHandlerContext context, ILifetimeScope scope)
        {
            Logger.LogDebug($"Handling apprenticeship Payment Order Changed Event For Account Id {message.AccountId} ");

            var processor = scope.Resolve<IApprenticeshipProcessor>();

            await processor.ProcessPaymentOrderChange(message);

            Logger.LogInfo($"Finished Handling apprenticeship Payment Order Changed Event For Account Id {message.AccountId} ");

        }
    }
}