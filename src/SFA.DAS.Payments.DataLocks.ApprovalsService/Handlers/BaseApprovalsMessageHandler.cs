using System;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Handlers
{
    public abstract class BaseApprovalsMessageHandler<T> : IHandleMessages<T>
    {
        protected IPaymentLogger Logger { get; private set; }
        private readonly IContainerScopeFactory factory;

        protected BaseApprovalsMessageHandler(IPaymentLogger logger, IContainerScopeFactory factory)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }


        public async Task Handle(T message, IMessageHandlerContext context)
        {
            Logger.LogVerbose($"Creating scope for handling message: {typeof(T).Name}");
            using (var scope = factory.CreateScope())
            {
                await HandleMessage(message, context, scope);
            }
            Logger.LogVerbose($"Finished handling message : {typeof(T).Name}");
        }

        protected abstract Task HandleMessage(T message, IMessageHandlerContext context, ILifetimeScope scope);
    }
}