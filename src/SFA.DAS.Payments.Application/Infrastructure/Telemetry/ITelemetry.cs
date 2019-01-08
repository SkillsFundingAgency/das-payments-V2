using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Application.Infrastructure.Telemetry
{
    public interface ITelemetry
    {
        void AddProperty(string propertyName, string value);
        void TrackEvent(string eventName);
        void TrackEvent(string eventName, double count);
        void TrackDuration(string durationName, TimeSpan duration);
        void TrackDependency(string dependencyType, string dependencyName, DateTimeOffset startTime, TimeSpan duration, bool success);
    }
}