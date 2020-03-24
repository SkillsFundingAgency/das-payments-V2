using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.UnitOfWork;

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
        private readonly string connectionString;
        public string EndpointName { get; set; }
        private readonly string errorQueueName;
        private CancellationToken startingCancellationToken;

        public StatelessServiceBusBatchCommunicationListener(string connectionString, string endpointName, string errorQueueName, IPaymentLogger logger,
            IContainerScopeFactory scopeFactory, ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
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

        private async Task<List<(Object Message, BatchMessageReceiver Receiver, Message ReceivedMessage)>> ReceiveMessages(BatchMessageReceiver messageReceiver, CancellationToken cancellationToken)
        {
            var applicationMessages = new List<(Object Message, BatchMessageReceiver Receiver, Message ReceivedMessage)>();
            var messages = await messageReceiver.ReceiveMessages(500, cancellationToken).ConfigureAwait(false);
            if (!messages.Any())
                return applicationMessages;

            foreach (var message in messages)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var applicationMessage = DeserializeMessage(message);
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

                        var messages = new List<(object Message, BatchMessageReceiver MessageReceiver, Message ReceivedMessage)>();
                        messages.AddRange(receiveTasks.SelectMany(task => task.Result));

                        if (!messages.Any())
                        {
                            await Task.Delay(1000, cancellationToken);
                            continue;
                        }
                        receiveTimer.Stop();
                        RecordMetric("StatelessServiceBusBatchCommunicationListener.ReceiveMessages", receiveTimer.ElapsedMilliseconds, messages.Count);

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
                        //RecordAllBatchProcessTelemetry(stopwatch.ElapsedMilliseconds, messages.Count);
                        pipeLineStopwatch.Stop();
                        //RecordPipelineTelemetry(pipeLineStopwatch.ElapsedMilliseconds, messages.Count);

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

        private void RecordBatchProcessTelemetry(long elapsedMilliseconds, int count)
        {
            RecordMetric("BatchedServiceBusCommunicationListener.ProcessBatches", elapsedMilliseconds, count);
        }

        private void RecordAllBatchProcessTelemetry(long elapsedMilliseconds, int count)
        {
            RecordMetric("BatchedServiceBusCommunicationListener.ProcessAllBatches", elapsedMilliseconds, count);
        }

        private void RecordPipelineTelemetry(long elapsedMilliseconds, int count)
        {
            RecordMetric("BatchedServiceBusCommunicationListener.Pipeline", elapsedMilliseconds, count);
        }

        private void RecordMetric(string eventName, long elapsedMilliseconds, int count)
        {
            var metrics = new Dictionary<string, double>
            {
                {TelemetryKeys.Duration, elapsedMilliseconds},
                {TelemetryKeys.Count, count}
            };
            telemetry.TrackEvent(eventName, metrics);
        }

        private object DeserializeMessage(Message message)
        {
            if (!message.UserProperties.ContainsKey(NServiceBus.Headers.EnclosedMessageTypes))
                throw new InvalidOperationException($"Cannot deserialise the message, no 'enclosed message types' header was found. Message id: {message.MessageId}, label: {message.Label}");
            var enclosedTypes = (string)message.UserProperties[NServiceBus.Headers.EnclosedMessageTypes];
            var typeName = enclosedTypes.Split(';').FirstOrDefault();
            if (string.IsNullOrEmpty(typeName))
                throw new InvalidOperationException($"Message type not found when trying to deserialise the message.  Message id: {message.MessageId}, label: {message.Label}");
            var messageType = Type.GetType(typeName, assemblyName => { assemblyName.Version = null; return Assembly.Load(assemblyName); },null );
            var sanitisedMessageJson = GetMessagePayload(message);
            var deserialisedMessage = JsonConvert.DeserializeObject(sanitisedMessageJson, messageType);
            return deserialisedMessage;
        }

        protected async Task ProcessMessages(Type groupType, List<(object Message, BatchMessageReceiver MessageReceiver, Message ReceivedMessage)> messages,
            CancellationToken cancellationToken)
        {
            try
            {
                using (var containerScope = scopeFactory.CreateScope())
                {
                    var stopwatch = Stopwatch.StartNew();
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
                    messages.Select(msg => msg.Message).ToList().ForEach(message => list.Add(message));

                    var handlerStopwatch = Stopwatch.StartNew();
                    await (Task)methodInfo.Invoke(handler, new object[] { list, cancellationToken });
                    RecordMetric(handler.GetType().FullName, handlerStopwatch.ElapsedMilliseconds, list.Count);
                    await Task.WhenAll(messages.GroupBy(msg => msg.MessageReceiver).Select(group =>
                        group.Key.Complete(group.Select(msg => msg.ReceivedMessage.SystemProperties.LockToken)))).ConfigureAwait(false);
                    RecordAllBatchProcessTelemetry(stopwatch.ElapsedMilliseconds, messages.Count);
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Error processing messages. Error: {e.Message}", e);
                await Task.WhenAll(messages.GroupBy(msg => msg.MessageReceiver).Select(group =>
                        group.Key.Abandon(group.Select(msg => msg.ReceivedMessage.SystemProperties.LockToken)
                            .ToList())))
                    .ConfigureAwait(false);
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

                logger.LogInfo($"Creating queue '{queuePath}' with properties: TimeToLive: 7 days, Lock Duration: 5 Minutes, Max Delivery Count: 50, Max Size: 5Gb.");
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
            catch (Exception e)
            {
                Console.WriteLine($"Error ensuring queue: {e.Message}.");
                Console.WriteLine(e);
                throw;
            }
        }
    }
}