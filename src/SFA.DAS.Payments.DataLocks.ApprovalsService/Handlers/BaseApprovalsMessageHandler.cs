using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Handlers
{
    public abstract class BaseApprovalsMessageHandler<T> : IHandleMessages<T>
    {
        protected IPaymentLogger Logger { get; private set; }
        private readonly ITelemetry telemetry;
        private readonly IContainerScopeFactory factory;

        protected BaseApprovalsMessageHandler(ITelemetry telemetry, IPaymentLogger logger, IContainerScopeFactory factory)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }


        public async Task Handle(T message, IMessageHandlerContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            Logger.LogVerbose($"Creating scope for handling message: {typeof(T).Name}");
            using (var scope = factory.CreateScope())
            {
                await HandleMessage(message, context, scope);
            }
            telemetry.TrackDuration($"DataLocks.ApprovalsService.{typeof(T).Name}", stopwatch.Elapsed);
            Logger.LogVerbose($"Finished handling message : {typeof(T).Name}");
        }

        protected abstract Task HandleMessage(T message, IMessageHandlerContext context, ILifetimeScope scope);
    }
}