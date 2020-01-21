using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using NServiceBus;
using NServiceBus.Features;

namespace SFA.DAS.Payments.Application.Infrastructure.Telemetry
{
    class NServiceBusApplicationInsightsFeature : Feature
    {
        public NServiceBusApplicationInsightsFeature()
        {
            Defaults(settings =>
            {
                metrics = settings.EnableMetrics();
            });
            EnableByDefault();
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            var settings = context.Settings;
            var logicalAddress = settings.LogicalAddress();
            var discriminator = logicalAddress.EndpointInstance.Discriminator;
            var instance = Guid.NewGuid().ToString("N");

            var endpoint = settings.EndpointName();
            var queue = settings.LocalAddress();

            collector = new ProbeCollector(
                endpoint,
                discriminator,
                instance,
                queue
            );

            metrics.RegisterObservers(collector.RegisterProbes);
            context.RegisterStartupTask(new CleanupAtStop(this));
        }

        class CleanupAtStop : FeatureStartupTask
        {
            public CleanupAtStop(NServiceBusApplicationInsightsFeature instance)
            {
                this.instance = instance;
            }

            protected override Task OnStart(IMessageSession session)
            {
                return Task.CompletedTask;
            }

            protected override Task OnStop(IMessageSession session)
            {
                instance.collector.Flush();
                return Task.CompletedTask;
            }

            NServiceBusApplicationInsightsFeature instance;
        }
        class ProbeCollector
        {
            readonly TelemetryClient telemetryClient;
            readonly Dictionary<string, string> probeNameToAiNameMap = new Dictionary<string, string>
        {
            {"# of msgs successfully processed / sec", "Success"},
            {"# of msgs pulled from the input queue /sec", "Fetched"},
            {"# of msgs failures / sec", "Failure"},
            {"Critical Time", "Critical Time (ms)"},
            {"Processing Time", "Processing Time (ms)"},
        };

            public ProbeCollector(string endpointName, string discriminator, string instanceIdentifier, string queue)
            {

                telemetryClient = new TelemetryClient();
                var properties = telemetryClient.Context.GlobalProperties;
                properties.Add("Endpoint", endpointName);
                properties.Add("EndpointInstance", instanceIdentifier);
                properties.Add("MachineName", Environment.MachineName);
                properties.Add("HostName", Dns.GetHostName());
                properties.Add("EndpointDiscriminator", discriminator);
                properties.Add("EndpointQueue", queue);
            }

            public void RegisterProbes(ProbeContext context)
            {

                foreach (var duration in context.Durations)
                {
                    if (!probeNameToAiNameMap.TryGetValue(duration.Name, out var name))
                    {
                        continue;
                    }
                    duration.Register(
                        observer: (ref DurationEvent durationEvent) =>
                        {
                            var milliseconds = durationEvent.Duration.TotalMilliseconds;
                            var telemetry = new MetricTelemetry(name, milliseconds);
                            telemetryClient.TrackMetric(telemetry);
                        });
                }

                foreach (var signal in context.Signals)
                {
                    if (!probeNameToAiNameMap.TryGetValue(signal.Name, out var name))
                    {
                        continue;
                    }
                    signal.Register(
                        observer: (ref SignalEvent signalEvent) =>
                        {
                            telemetryClient.TrackEvent(new EventTelemetry(name));
                        });
                }

            }

            public void Flush()
            {
                telemetryClient.Flush();
            }
        }

        MetricsOptions metrics;
        ProbeCollector collector;
    }
}
