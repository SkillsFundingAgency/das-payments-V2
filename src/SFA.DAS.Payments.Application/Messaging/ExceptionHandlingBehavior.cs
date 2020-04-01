using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using NServiceBus.Transport;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;

namespace SFA.DAS.Payments.Application.Messaging
{
    public class ExceptionHandlingBehavior : Behavior<IIncomingLogicalMessageContext>
    {
        private readonly IPaymentLogger logger;

        public ExceptionHandlingBehavior(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            try
            {
                await next().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!(context.Headers.TryGetValue("NServiceBus.EnclosedMessageTypes", out var messageType) 
                    && TypeString.TryParseTypeName(messageType, out messageType)))
                {
                    messageType = "No enclosed messages";
                }

                if (!context.Headers.TryGetValue("NServiceBus.OriginatingEndpoint", out var sendingEndpoint))
                    sendingEndpoint = "No sending endpoint";

                var add = AdditionalProperties(context.Message);
                var props = string.Join(", ", add.Select(x => $"{x.Key}: {x.Value}"));

                throw new MessageProcessingFailedException($"Couldn't process message `{messageType}` from `{sendingEndpoint}`.\n{props}\n{ex.Message}", context.Message, ex);
            }
        }

        private Dictionary<string,string> AdditionalProperties(LogicalMessage message)
        {
            var props = new List<string> { "Id", "EmployerAccountId", "AccountId", "Ukprn", "JobId", "EventId", "ContractType", "TransactionType" };

            return props.ToDictionaryWithoutNullValues(key => key, key => GetMessageProperty(message, key));
        }

        private static string GetMessageProperty(LogicalMessage message, string propertyName)
        {
            return message.Instance.GetType().GetProperty(propertyName)?.GetValue(message.Instance, null)?.ToString();
        }
    }

    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> ToDictionaryWithoutNullValues<TSource, TKey, TValue>(this IEnumerable<TSource> keys, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            // message.Learner?.ReferenceNumber
            // message.Learner.LearnRefNumber
            //{message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear:0000}"
             
            return keys
                .Select(x => (key: keySelector(x), value: valueSelector(x)))
                .Where(x => x.value != null)
                .ToDictionary(x => x.key, x => x.value);
        }
    }

}