﻿using System;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure
{
    public static class Extensions
    {
        public static async Task<bool> DeferDueToJobNotFound(this IMessageHandlerContext context, object message, int delayInSeconds)
        {
            return await Defer(context, message, TimeSpan.FromSeconds(delayInSeconds), "JobNotFoundRetries");
        }

        public static async Task<bool> DeferDueToUpdateException(this IMessageHandlerContext context, object message, int delayInSeconds)
        {
            return await Defer(context, message, TimeSpan.FromSeconds(delayInSeconds), "JobUpdateFailedRetries");
        }

        public static async Task<bool> Defer(this IMessageHandlerContext context, object message, TimeSpan delay, string retriesKey, int maxnumberOfRetries = 5)
        {
            var retriesHeader = context.MessageHeaders.ContainsKey(retriesKey) ? context.MessageHeaders[retriesKey] : null;
            var retries = string.IsNullOrEmpty(retriesHeader) ? 0 : int.Parse(retriesHeader);
            if (++retries > maxnumberOfRetries)
            {
                return false;
            }

            var options = new SendOptions();
            options.DelayDeliveryWith(delay);
            options.SetHeader(retriesKey, retries.ToString());
            await context.Send(message, options);
            context.DoNotContinueDispatchingCurrentMessageToHandlers();
            return true;
        }
    }
}