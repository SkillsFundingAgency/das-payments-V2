using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace SFA.DAS.Payments.Application.Infrastructure.Telemetry
{
    public interface ITelemetry
    {
        void AddProperty(string propertyName, string value);
        void TrackEvent(string eventName);
        void TrackEvent(string eventName, double count);
        void TrackEvent(string eventName, Dictionary<string, double> metrics);
        void TrackEvent(string eventName, Dictionary<string, string> properties, Dictionary<string, double> metrics);
        void TrackDuration(string durationName, TimeSpan duration);
        void TrackDependency(string dependencyType, string dependencyName, DateTimeOffset startTime, TimeSpan duration, bool success);
        IOperationHolder<RequestTelemetry> StartOperation(string operationName = "PaymentMessageProcessing");
        void StopOperation(IOperationHolder<RequestTelemetry> operation); 
    }
}