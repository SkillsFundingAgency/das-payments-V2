using System;
using System.Collections.Generic;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Core.Configuration;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Infrastructure
{
    [Binding]
    public class MetricsBindings : TestSessionBase
    {
        public MetricsBindings(FeatureContext context) : base(context)
        {
        }

        private static readonly string TestRunId = Guid.NewGuid().ToString("N");
        private static DateTime testRunStartTime;
        private static int numberOfFeatures;
        private static int numberOfScenarios;
        private static readonly object LockObject = new object();

        [BeforeTestRun(Order = 3)]
        public static void SetUpContainer()
        {
            Builder.RegisterType<TestConfigurationHelper>().As<IConfigurationHelper>().SingleInstance();
            Builder.RegisterModule<TelemetryModule>();
            testRunStartTime = DateTime.Now;
        }

        [BeforeTestRun(Order = 51)]
        public static void RecordStartTime()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                var telemetry = scope.Resolve<ITelemetry>();
                telemetry.TrackEvent("Starting Test Run",
                    new Dictionary<string, string> {
                        { "Type", "TestRun" },
                        { "StartTime", testRunStartTime.ToString("O") },
                        { "TestRunId", TestRunId}
                    },
                    new Dictionary<string, double>());
            }
        }

        [AfterTestRun(Order = 1)]
        public static void ReportTestRunDuration()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                var telemetry = scope.Resolve<ITelemetry>();
                var duration = DateTime.Now - testRunStartTime;
                telemetry.TrackEvent("Finished Test Run",
                    new Dictionary<string, string> {
                        { "Type", "TestRun" },
                        { "StartTime", testRunStartTime.ToString("O") },
                        { "TestRunId", TestRunId}
                    },
                    new Dictionary<string, double>
                    {
                        { TelemetryKeys.Duration, duration.TotalMilliseconds },
                        { "DurationInSeconds", duration.TotalSeconds },
                        { "NumberOfFeatures", numberOfFeatures }, //no need to lock here
                        { "NumberOfScenarios", numberOfScenarios } //no need to lock here
                    }
                );
            }
        }

        [BeforeFeature(Order = 1)]
        public static void RecordFeatureStartTime(FeatureContext context)
        {
            var telemetry = context.Get<ILifetimeScope>("container_scope")
                .Resolve<ITelemetry>();
            telemetry.TrackEvent($"Starting Feature: {context.FeatureInfo.Title}");
            context.Set(DateTime.Now, "feature_start_time");
            context.Set(0, "number_of_scenarios_in_feature");
        }

        [AfterFeature(Order = 99)]
        public static void ReportFeatureDuration(FeatureContext context)
        {
            var startTime = context.Get<DateTime>("feature_start_time");
            var numberOfScenariosInFeature = context.Get<int>("number_of_scenarios_in_feature");
            var duration = DateTime.Now - startTime;
            var telemetry = context.Get<ILifetimeScope>("container_scope").Resolve<ITelemetry>();
            var testSession = context.Get<TestSession>();

            telemetry.TrackEvent($"Finished Feature: {context.FeatureInfo.Title} with Ukprn: {testSession.Ukprn} Job ID: {testSession.JobId}",
                new Dictionary<string, string>
                {
                    { "Type", "Feature" },
                    { "Feature", context.FeatureInfo.Title },
                    { "StartTime", startTime.ToString("O") },
                    { "TestRunId", TestRunId}
                },
                new Dictionary<string, double>
                {
                    { TelemetryKeys.Duration, duration.TotalMilliseconds },
                    { "NumberOfScenarios", numberOfScenariosInFeature }
                }
            );
            lock (LockObject)
            {
                numberOfFeatures++;
                numberOfScenarios += numberOfScenariosInFeature;
            }
        }

        [BeforeScenario]
        public void RecordScenarioStartTime(ScenarioContext context)
        {
            context.Set<DateTime>(DateTime.Now, "scenario_start_time");
        }

        [AfterScenario]
        public void ReportScenarioDuration(ScenarioContext context)
        {
            var startTime = context.Get<DateTime>("scenario_start_time");
            var duration = DateTime.Now - startTime;
            var telemetry = Scope.Resolve<ITelemetry>();
            telemetry.TrackEvent($"Finished Scenario: {context.ScenarioInfo.Title}",
                new Dictionary<string, string>
                {
                    { "Type", "Scenario" },
                    { "Scenario", context.ScenarioInfo.Title },
                    { "Feature", (Context as FeatureContext)?.FeatureInfo.Title },
                    { "Status", context.TestError != null ? "Failed" : "Success" },
                    { "StartTime", startTime.ToString("O") },
                    { "TestRunId", TestRunId}
                },
                new Dictionary<string, double> { { TelemetryKeys.Duration, duration.TotalMilliseconds } }
            );
            lock (LockObject)
            {
                var scenarioCount = Context.Get<int>("number_of_scenarios_in_feature");
                scenarioCount++;
                Context.Set(scenarioCount, "number_of_scenarios_in_feature");
            }
        }

        [BeforeStep]
        public void RecordStepStartTime(ScenarioContext context)
        {
            context.Set<DateTime>(DateTime.Now, "step_start_time");
        }

        [AfterStep]
        public void ReportStepDuration(ScenarioContext scenarioContext)
        {
            var startTime = scenarioContext.Get<DateTime>("step_start_time");
            var context = scenarioContext.StepContext;
            var duration = DateTime.Now - startTime;
            var telemetry = Scope.Resolve<ITelemetry>();
            telemetry.TrackEvent($"Finished Step: {context.StepInfo.StepDefinitionType:G} {context.StepInfo.Text}",
                new Dictionary<string, string> {
                    { "Type", "Step" },
                    { "Scenario", context.StepInfo.StepInstance.StepContext.ScenarioTitle },
                    { "Feature", context.StepInfo.StepInstance.StepContext.FeatureTitle },
                    { "Step", context.StepInfo.StepInstance.Text },
                    { "StepType", context.StepInfo.StepDefinitionType.ToString("G") },
                    { "StartTime", startTime.ToString("O") },
                    { "TestRunId", TestRunId}
                },
                new Dictionary<string, double> { { TelemetryKeys.Duration, duration.TotalMilliseconds } }
            );
        }
    }
}