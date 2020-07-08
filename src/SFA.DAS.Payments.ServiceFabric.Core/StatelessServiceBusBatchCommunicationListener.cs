using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Autofac;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Messaging.Serialization;

namespace SFA.DAS.Payments.ServiceFabric.Core
{

    public interface IStatelessServiceBusBatchCommunicationListener : ICommunicationListener
    {
        string EndpointName { get; set; }
    }

    public class StatelessServiceBusBatchCommunicationListener : IStatelessServiceBusBatchCommunicationListener
    {
        private readonly IPaymentLogger logger;
        private readonly IContainerScopeFactory scopeFactory;
        private readonly ITelemetry telemetry;
        private readonly IMessageDeserializer messageDeserializer;
        private readonly IApplicationMessageModifier messageModifier;
        private readonly string connectionString;
        public string EndpointName { get; set; }
        private readonly string errorQueueName;
        private CancellationToken startingCancellationToken;
        protected string TelemetryPrefix => GetType().Name;

        private const string TopicPath = "bundle-1";

        public StatelessServiceBusBatchCommunicationListener(string connectionString, string endpointName, string errorQueueName, IPaymentLogger logger,
            IContainerScopeFactory scopeFactory, ITelemetry telemetry, IMessageDeserializer messageDeserializer, IApplicationMessageModifier messageModifier)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            this.messageDeserializer = messageDeserializer ?? throw new ArgumentNullException(nameof(messageDeserializer));
            this.messageModifier = messageModifier ?? throw new ArgumentNullException(nameof(messageModifier));
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            EndpointName = endpointName ?? throw new ArgumentNullException(nameof(endpointName));
            this.errorQueueName = errorQueueName ?? endpointName + "-Errors";
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            startingCancellationToken = cancellationToken;
            _ = ListenForMessages(cancellationToken);
            return Task.FromResult(EndpointName);
        }

        protected virtual async Task ListenForMessages(CancellationToken cancellationToken)
        {
            await EnsureQueue(EndpointName).ConfigureAwait(false);
            await EnsureSubscriptions(EndpointName, cancellationToken).ConfigureAwait(false);
            await EnsureQueue(errorQueueName).ConfigureAwait(false);
            try
            {
                await Listen(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogFatal($"Encountered fatal error. Error: {ex.Message}", ex);
            }
        }

        private async Task EnsureSubscriptions(string endpointName, CancellationToken cancellationToken)
        {
            try
            {
                //get class names that are subscribing to IHandleBatchMessages
                List<Type> subscribedMessageTypes = GetBatchHandledMessageTypes();
                if (!subscribedMessageTypes.Any()) return;

                //GetCurrentSubscriptions
                _ = await GetOrCreateSubscription(endpointName, cancellationToken);

                var existingRules = await GetExistingRules(endpointName, cancellationToken);

                foreach (var type in subscribedMessageTypes)
                {
                    if (!existingRules.Any(x => x.Name == type.Name))
                    {
                        CreateNewSubscriptionRule(type, endpointName, cancellationToken);
                    }
                }
            }
            catch (MessagingEntityAlreadyExistsException ex)
            {
                logger.LogInfo($"The message queue entity already exists: {ex.Message}. This could be because another instance of the service has already ensured the entity exists");
            }
            catch (Exception e)
            {
                logger.LogFatal($"Error ensuring subscription, or rule: {e.Message}.", e);
                throw;
            }
        }

        private void CreateNewSubscriptionRule(Type type, string endpointName, CancellationToken cancellationToken)
        {
            var manageClient = new ManagementClient(connectionString);
            var ruleDescription = new RuleDescription
            {
                Filter = new SqlFilter($"[NServiceBus.EnclosedMessageTypes] LIKE '%{type.FullName}%'"),
                Name = type.Name
            };

            manageClient.CreateRuleAsync(TopicPath, endpointName, ruleDescription, cancellationToken);
        }

        private async Task<IList<RuleDescription>> GetExistingRules(string subscriptionName, CancellationToken cancellationToken)
        {
            var manageClient = new ManagementClient(connectionString);
            return await manageClient.GetRulesAsync(TopicPath, subscriptionName, cancellationToken: cancellationToken);
        }

        private async Task<SubscriptionDescription> GetOrCreateSubscription(string endpointName, CancellationToken cancellationToken)
        {
            var manageClient = new ManagementClient(connectionString);

            SubscriptionDescription subscriptionDescription;
            if (!await manageClient.SubscriptionExistsAsync(TopicPath, endpointName, cancellationToken))
            {
                subscriptionDescription = new SubscriptionDescription(TopicPath, endpointName)
                {
                    ForwardTo = endpointName,
                    UserMetadata = endpointName,
                    EnableBatchedOperations = true,
                    MaxDeliveryCount = Int32.MaxValue,
                    EnableDeadLetteringOnFilterEvaluationExceptions = false,
                    LockDuration = TimeSpan.FromMinutes(5)
                };
                var defaultRule = new RuleDescription("$default") { Filter = new SqlFilter("1=0") };
                await manageClient.CreateSubscriptionAsync(
                   subscriptionDescription, defaultRule, cancellationToken);
            }
            else
            {
                subscriptionDescription =
                    await manageClient.GetSubscriptionAsync(TopicPath, endpointName, cancellationToken);
            }

            return subscriptionDescription;
        }

        private List<Type> GetBatchHandledMessageTypes()
        {
            List<Type> genericTypes = new List<Type>();

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes());

            foreach (var type in types)
            {
                foreach (Type intType in type.GetInterfaces())
                {
                    if (intType.IsGenericType && intType.GetGenericTypeDefinition()
                        == typeof(IHandleMessageBatches<>))
                    {
                        genericTypes.Add(intType.GetGenericArguments()[0]);
                    }
                }
            }

            return genericTypes;
        }

        private async Task<List<(Object Message, BatchMessageReceiver Receiver, Message ReceivedMessage)>> ReceiveMessages(BatchMessageReceiver messageReceiver, CancellationToken cancellationToken)
        {
            var applicationMessages = new List<(Object Message, BatchMessageReceiver Receiver, Message ReceivedMessage)>();
            var messages = await messageReceiver.ReceiveMessages(200, cancellationToken).ConfigureAwait(false);
            if (!messages.Any())
                return applicationMessages;

            foreach (var message in messages)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var applicationMessage = GetApplicationMessage(message);
                    applicationMessages.Add((applicationMessage, messageReceiver, message));
                }
                catch (Exception e)
                {
                    logger.LogError($"Error deserialising the message. Error: {e.Message}", e);
                    //TODO: should use the error queue instead of dead letter queue
                    await messageReceiver.DeadLetter(message)
                        .ConfigureAwait(false);
                }
            }

            return applicationMessages;
        }

        private async Task Listen(CancellationToken cancellationToken)
        {
            var connection = new ServiceBusConnection(connectionString);
            var messageReceivers = new List<BatchMessageReceiver>();
            messageReceivers.AddRange(Enumerable.Range(0, 3)
                .Select(i => new BatchMessageReceiver(connection, EndpointName)));
            var errorQueueSender = new MessageSender(connection, errorQueueName, RetryPolicy.Default);
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var pipeLineStopwatch = Stopwatch.StartNew();
                        var receiveTimer = Stopwatch.StartNew();

                        var receiveTasks =
                            messageReceivers.Select(receiver => ReceiveMessages(receiver, cancellationToken)).ToList();
                        await Task.WhenAll(receiveTasks).ConfigureAwait(false);

                        var messages = receiveTasks.SelectMany(task => task.Result).ToList();
                        receiveTimer.Stop();
                        if (!messages.Any())
                        {
                            await Task.Delay(2000, cancellationToken);
                            continue;
                        }
                        RecordMetric("ReceiveMessages", receiveTimer.ElapsedMilliseconds, messages.Count);
                        var groupedMessages = new Dictionary<Type, List<(object Message, BatchMessageReceiver MessageReceiver, Message ReceivedMessage)>>();
                        foreach (var message in messages)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            var key = message.Message.GetType();
                            var applicationMessages = groupedMessages.ContainsKey(key)
                                ? groupedMessages[key]
                                : groupedMessages[key] = new List<(object Message, BatchMessageReceiver MessageReceiver, Message ReceivedMessage)>();
                            applicationMessages.Add(message);
                        }

                        var stopwatch = Stopwatch.StartNew();
                        await Task.WhenAll(groupedMessages.Select(group =>
                            ProcessMessages(group.Key, group.Value, cancellationToken)));
                        stopwatch.Stop();
                        RecordProcessedAllBatchesTelemetry(stopwatch.ElapsedMilliseconds, messages.Count);
                        pipeLineStopwatch.Stop();
                        RecordPipelineTelemetry(pipeLineStopwatch.ElapsedMilliseconds, messages.Count);
                    }
                    catch (TaskCanceledException)
                    {
                        logger.LogWarning("Cancelling communication listener.");
                        return;
                    }
                    catch (OperationCanceledException)
                    {
                        logger.LogWarning("Cancelling communication listener.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Error listening for message.  Error: {ex.Message}", ex);
                    }
                }
            }
            finally
            {
                await Task.WhenAll(messageReceivers.Select(receiver => receiver.Close())).ConfigureAwait(false);
                if (!connection.IsClosedOrClosing)
                    await connection.CloseAsync();
            }
        }

        private void RecordProcessedBatchTelemetry(long elapsedMilliseconds, int count, string batchType)
        {
            RecordMetric("ProcessedBatch", elapsedMilliseconds, count, (properties, metrics) => properties.Add("MessageBatchType", batchType));
        }

        private void RecordProcessedAllBatchesTelemetry(long elapsedMilliseconds, int count)
        {
            RecordMetric("ProcessedAllBatches", elapsedMilliseconds, count);
        }

        private void RecordPipelineTelemetry(long elapsedMilliseconds, int count)
        {
            RecordMetric("Pipeline", elapsedMilliseconds, count);
        }

        private void RecordMetric(string eventName, long elapsedMilliseconds, int count, Action<Dictionary<string, string>, Dictionary<string, double>> metricsAction = null)
        {
            var metrics = new Dictionary<string, double>
            {
                {TelemetryKeys.Duration, elapsedMilliseconds},
                {TelemetryKeys.Count, count}
            };
            var properties = new Dictionary<string, string>();
            metricsAction?.Invoke(properties, metrics);
            telemetry.TrackEvent($"{TelemetryPrefix}.{eventName}", properties, metrics);
        }

        private object GetApplicationMessage(Message message)
        {
            var applicationMessage = DeserializeMessage(message);
            return messageModifier.Modify(applicationMessage);
        }

        private object DeserializeMessage(Message message)
        {
            return messageDeserializer.DeserializeMessage(message);
        }

        protected async Task ProcessMessages(Type groupType, List<(object Message, BatchMessageReceiver MessageReceiver, Message ReceivedMessage)> messages,
            CancellationToken cancellationToken)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                using (var containerScope = scopeFactory.CreateScope())
                {
                    if (!containerScope.TryResolve(typeof(IHandleMessageBatches<>).MakeGenericType(groupType),
                        out object handler))
                    {
                        logger.LogError($"No handler found for message: {groupType.FullName}");
                        await Task.WhenAll(messages.Select(message => message.MessageReceiver.DeadLetter(message.ReceivedMessage)));
                        return;
                    }

                    var methodInfo = handler.GetType().GetMethod("Handle");
                    if (methodInfo == null)
                        throw new InvalidOperationException($"Handle method not found on handler: {handler.GetType().Name} for message type: {groupType.FullName}");

                    var listType = typeof(List<>).MakeGenericType(groupType);
                    var list = (IList)Activator.CreateInstance(listType);
                    messages.ForEach(message => list.Add(message.Message));

                    var handlerStopwatch = Stopwatch.StartNew();
                    await (Task)methodInfo.Invoke(handler, new object[] { list, cancellationToken });
                    RecordMetric(handler.GetType().FullName, handlerStopwatch.ElapsedMilliseconds, list.Count);
                    await Task.WhenAll(messages.GroupBy(msg => msg.MessageReceiver).Select(group =>
                        group.Key.Complete(group.Select(msg => msg.ReceivedMessage.SystemProperties.LockToken)))).ConfigureAwait(false);
                }
                RecordProcessedBatchTelemetry(stopwatch.ElapsedMilliseconds, messages.Count, groupType.FullName);
            }
            catch (Exception e)
            {
                logger.LogError($"Error processing messages. Error: {e.Message}", e);
                await Task.WhenAll(messages.Where(msg => msg.ReceivedMessage.SystemProperties.DeliveryCount < 10).GroupBy(msg => msg.MessageReceiver).Select(group =>
                        group.Key.Abandon(group.Select(msg => msg.ReceivedMessage.SystemProperties.LockToken)
                            .ToList())))
                    .ConfigureAwait(false);
                await RetryFailedMessages(groupType, messages.Where(msg => msg.ReceivedMessage.SystemProperties.DeliveryCount >= 10).ToList(), cancellationToken);
            }
        }

        protected async Task RetryFailedMessages(Type groupType,
            List<(object Message, BatchMessageReceiver MessageReceiver, Message ReceivedMessage)> messages,
            CancellationToken cancellationToken)
        {
            var listType = typeof(List<>).MakeGenericType(groupType);
            var list = (IList)Activator.CreateInstance(listType);
            foreach (var retryMessage in messages)
            {
                try
                {
                    using (var scope = scopeFactory.CreateScope())
                    {
                        if (!scope.TryResolve(typeof(IHandleMessageBatches<>).MakeGenericType(groupType),
                            out object handler))
                        {
                            logger.LogError($"No handler found for message: {groupType.FullName}");
                            await Task.WhenAll(messages.Select(message => message.MessageReceiver.DeadLetter(message.ReceivedMessage)));
                            return;
                        }

                        var methodInfo = handler.GetType().GetMethod("Handle");
                        if (methodInfo == null)
                            throw new InvalidOperationException($"Handle method not found on handler: {handler.GetType().Name} for message type: {groupType.FullName}");

                        list.Clear();
                        list.Add(retryMessage.Message);

                        var handlerStopwatch = Stopwatch.StartNew();
                        await (Task)methodInfo.Invoke(handler, new object[] { list, cancellationToken });
                        RecordMetric($"{handler.GetType().FullName}:Single", handlerStopwatch.ElapsedMilliseconds, 1);

                        await retryMessage.MessageReceiver.Complete(retryMessage.ReceivedMessage.SystemProperties.LockToken);
                    }
                }
                catch (Exception e)
                {
                    logger.LogError($"Error processing message.  Error: {e.Message}.  ASB Message id: {retryMessage.ReceivedMessage.MessageId}, Message label: {retryMessage.ReceivedMessage.Label}.", e);
                    await retryMessage.MessageReceiver.Abandon(retryMessage.ReceivedMessage.SystemProperties.LockToken);
                }
            }
        }

        private string GetMessagePayload(Message receivedMessage)
        {
            const string transportEncodingHeaderKey = "NServiceBus.Transport.Encoding";
            var transportEncoding = receivedMessage.UserProperties.ContainsKey(transportEncodingHeaderKey)
                ? (string)receivedMessage.UserProperties[transportEncodingHeaderKey]
                : "application/octet-stream";
            byte[] messageBody;
            if (transportEncoding.Equals("wcf/byte-array", StringComparison.OrdinalIgnoreCase))
            {
                var doc = receivedMessage.GetBody<XmlElement>();
                messageBody = Convert.FromBase64String(doc.InnerText);
            }
            else
                messageBody = receivedMessage.Body;

            var monitoringMessageJson = Encoding.UTF8.GetString(messageBody);
            var sanitisedMessageJson = monitoringMessageJson
                .Trim(Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble())
                    .ToCharArray());
            return sanitisedMessageJson;
        }

        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            if (!startingCancellationToken.IsCancellationRequested)
                startingCancellationToken = cancellationToken;
        }

        public void Abort()
        {
        }

        private async Task EnsureQueue(string queuePath)
        {
            try
            {
                var manageClient = new ManagementClient(connectionString);
                if (await manageClient.QueueExistsAsync(queuePath, startingCancellationToken).ConfigureAwait(false))
                {
                    logger.LogInfo($"Queue '{queuePath}' already exists, skipping queue creation.");
                    return;
                }

                logger.LogInfo(
                    $"Creating queue '{queuePath}' with properties: TimeToLive: 7 days, Lock Duration: 5 Minutes, Max Delivery Count: 50, Max Size: 5Gb.");
                var queueDescription = new QueueDescription(queuePath)
                {
                    DefaultMessageTimeToLive = TimeSpan.FromDays(7),
                    EnableDeadLetteringOnMessageExpiration = true,
                    LockDuration = TimeSpan.FromMinutes(5),
                    MaxDeliveryCount = 50,
                    MaxSizeInMB = 5120,
                    Path = queuePath
                };

                await manageClient.CreateQueueAsync(queueDescription, startingCancellationToken).ConfigureAwait(false);
            }
            catch (MessagingEntityAlreadyExistsException ex)
            {
                logger.LogInfo($"Queue already exists: {ex.Message}. This could be because another instance of the service has already ensured the queue exists");
            }
            catch (Exception e)
            {
                logger.LogFatal($"Error ensuring queue: {e.Message}.", e);
                throw;
            }
        }
    }
}