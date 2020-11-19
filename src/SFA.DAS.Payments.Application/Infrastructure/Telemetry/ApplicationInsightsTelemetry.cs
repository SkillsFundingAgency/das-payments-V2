using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Newtonsoft.Json;

namespace SFA.DAS.Payments.Application.Infrastructure.Telemetry
{
    public class ApplicationInsightsTelemetry : ITelemetry, IDisposable
    {
        private TelemetryClient telemetryClient;
        private readonly Dictionary<string, string> properties;
        public ApplicationInsightsTelemetry(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            properties = new Dictionary<string, string>
            {
                { "MachineName", Environment.MachineName },
                { "ProcessName", System.Diagnostics.Process.GetCurrentProcess().ProcessName },
                { "ThreadId", Environment.CurrentManagedThreadId.ToString() }

            };
        }

        public void TrackEvent(string eventName)
        {
            telemetryClient.TrackEvent($"Event: {eventName}", properties);
        }

        public void TrackEvent(string eventName, double count)
        {
            telemetryClient.TrackEvent($"Event: {eventName}", properties, new Dictionary<string, double> { { TelemetryKeys.Count, count } });
        }

        public void TrackEvent(string eventName, Dictionary<string, double> metrics)
        {
            telemetryClient.TrackEvent($"Event: {eventName}", properties, metrics);
        }

        public void TrackEvent(string eventName, Dictionary<string, string> eventProperties, Dictionary<string, double> metrics)
        {

            foreach (var property in properties)
            {
                if (!eventProperties.ContainsKey(property.Key))
                    eventProperties.Add(property.Key, property.Value);
            }
            telemetryClient.TrackEvent($"Event: {eventName}", eventProperties, metrics);
        }

        public void TrackDuration(string durationName, TimeSpan duration)
        {
            telemetryClient.TrackEvent($"Event: {durationName}", properties, new Dictionary<string, double> { { TelemetryKeys.Duration, duration.TotalMilliseconds } });
        }

        public void TrackDependency(string dependencyType, string dependencyName, DateTimeOffset startTime, TimeSpan duration, bool success)
        {
            telemetryClient.TrackDependency(dependencyType, $"Dependency: {dependencyName}", JsonConvert.SerializeObject(properties), startTime, duration, success);
        }

        public IOperationHolder<RequestTelemetry> StartOperation(string operationName = "PaymentMessageProcessing", string operationId = null)
        {
            return telemetryClient.StartOperation<RequestTelemetry>(operationName, operationId);
        }

        public void StopOperation(IOperationHolder<RequestTelemetry> operation)
        {
            telemetryClient.StopOperation(operation);
        }

        private void ReleaseUnmanagedResources()
        {
            telemetryClient?.Flush();
            telemetryClient = null;
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~ApplicationInsightsTelemetry()
        {
            ReleaseUnmanagedResources();
        }
    }
}