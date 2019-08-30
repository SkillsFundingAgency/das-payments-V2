using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Handlers
{
    public abstract class BaseApprovalsMessageHandler<T> : IHandleMessages<T> where T : class
    {
        private readonly IPeriodEndEventRepository periodEndEventRepository;
        private readonly IDeferredApprovalsEventRepository deferredApprovalsEventRepository;
        private readonly TimeSpan periodEndCheckInterval;

        private static readonly string PeriodEndStartedEventName = typeof(PeriodEndStartedEvent).Name;
        private static readonly string PeriodEndRunningEventName = typeof(PeriodEndRunningEvent).Name;
        private static DateTime lastPeriodEndCheck;
        private static bool? lastCheck;

        protected IPaymentLogger Logger { get; }
        private readonly IContainerScopeFactory factory;

        protected BaseApprovalsMessageHandler(IPaymentLogger logger, IContainerScopeFactory factory, IPeriodEndEventRepository periodEndEventRepository, IDeferredApprovalsEventRepository deferredApprovalsEventRepository, IConfigurationHelper configHelper)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.periodEndEventRepository = periodEndEventRepository;
            this.deferredApprovalsEventRepository = deferredApprovalsEventRepository;
            periodEndCheckInterval = TimeSpan.FromMinutes(double.Parse(configHelper.GetSetting("PeriodEndStatusCheckIntervalInMinutes") ?? "10"));
        }

        public async Task Handle(T message, IMessageHandlerContext context)
        {
            try
            {
                Logger.LogVerbose($"Creating scope for handling message: {typeof(T).Name}");
                using (var scope = factory.CreateScope())
                {
                    if (await CanHandle(context).ConfigureAwait(false))
                        await HandleMessage(message, context, scope).ConfigureAwait(false);
                    else
                        await Defer(message).ConfigureAwait(false);
                }
                Logger.LogVerbose($"Finished handling message : {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error handling {typeof(T).Name} message. Error: {ex.Message}", ex);
                throw;
            }
        }

        private async Task<bool> CanHandle(IMessageHandlerContext context)
        {
            if (IsDeferred(context)) // deferred messages also come here when period end stops
                return true;

            if (lastCheck.HasValue && DateTime.UtcNow.Subtract(lastPeriodEndCheck) < periodEndCheckInterval)
                return lastCheck.Value;

            Logger.LogVerbose("Reading latest period end event from DB");

            var lastPeriodEndEvent = await periodEndEventRepository.GetLastPeriodEndEvent(CancellationToken.None).ConfigureAwait(false);

            if (lastPeriodEndEvent == null)
            {
                lastCheck = true;
            }
            else if (lastPeriodEndEvent.EventType == PeriodEndStartedEventName || lastPeriodEndEvent.EventType == PeriodEndRunningEventName)
            {
                Logger.LogDebug($"Approvals message deferred as last period end event is {lastPeriodEndEvent.EventType}");
                lastCheck = false;
            }
            else
            {
                lastCheck = true;
                // if period end stopped but there are still messages to process - still defer

            }

            lastPeriodEndCheck = DateTime.UtcNow;
            return lastCheck.Value;
        }

        private async Task Defer(T message)
        {
            var deferredApprovalsEventEntity = new DeferredApprovalsEventEntity
            {
                EventTime = DateTime.UtcNow,
                ApprovalsEvent = message
            };
            await deferredApprovalsEventRepository.StoreDeferredEvent(deferredApprovalsEventEntity, CancellationToken.None).ConfigureAwait(false);
        }

        private bool IsDeferred(IMessageHandlerContext context)
        {
            return context.MessageHeaders.ContainsKey(PeriodEndStoppedEventHandler.DeferredMessageIdHeader);
        }

        protected abstract Task HandleMessage(T message, IMessageHandlerContext context, ILifetimeScope scope);
    }
}