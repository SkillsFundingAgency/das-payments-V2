using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.Application.Messaging
{
    public class ExceptionHandlingBehavior : Behavior<ITransportReceiveContext>
    {
        private readonly IPaymentLogger logger;

        public ExceptionHandlingBehavior(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task Invoke(ITransportReceiveContext context, Func<Task> next)
        {
            try
            {
                await next().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError($"Message handling failed. Error: {ex.Message}", ex);
                throw;
            }
        }
    }
}