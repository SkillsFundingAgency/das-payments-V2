using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using FluentAssertions;
using NUnit.Framework;
using Polly;
using Polly.Registry;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.ComparisonTesting;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure;
using SFA.DAS.Payments.AcceptanceTests.Services;
using SFA.DAS.Payments.AcceptanceTests.Services.BespokeHttpClient;
using SFA.DAS.Payments.AcceptanceTests.Services.Configuration;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification

{
    [Category("verification_ete")]
    public class ETEVerificationTests
    {
        private IContainer autofacContainer;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<CloudStorageSettings>().SingleInstance();
            builder.RegisterType<TestOrchestrator>().As<ITestOrchestrator>().InstancePerLifetimeScope();
            builder.RegisterType<VerificationService>().As<IVerificationService>().InstancePerLifetimeScope();
            builder.RegisterType<JobService>().As<IJobService>().InstancePerLifetimeScope();

            builder.RegisterType<BespokeHttpClient>().As<IBespokeHttpClient>().InstancePerLifetimeScope();
            builder.RegisterType<AzureStorageServiceConfig>()
                .As<IAzureStorageKeyValuePersistenceServiceConfig>().InstancePerLifetimeScope();
            builder.RegisterType<AzureStorageKeyValuePersistenceService>()
                .As<IStreamableKeyValuePersistenceService>().InstancePerLifetimeScope();
            builder.RegisterType<SubmissionService>().As<ISubmissionService>().InstancePerLifetimeScope();

            builder.Register(context =>
                {
                    var registry = new PolicyRegistry();
                    registry.Add(
                        "HttpRetryPolicy",
                        Policy.Handle<HttpRequestException>()
                            .WaitAndRetryAsync(
                                3, // number of retries
                                retryAttempt =>
                                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // exponential backoff
                                (exception, timeSpan, retryCount, executionContext) =>
                                {
                                    // add logging
                                }));
                    return registry;
                }).As<IReadOnlyPolicyRegistry<string>>()
                .SingleInstance();

            builder.RegisterType<JobService>().As<IJobService>().InstancePerLifetimeScope();
            builder.RegisterType<BespokeHttpClient>().As<IBespokeHttpClient>().InstancePerLifetimeScope();

            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>()
                .InstancePerLifetimeScope();
            var container = builder.Build();

            autofacContainer = container;
        }

        [Test]
        public async Task InitialTest()
        {
            // Arrange
            DateTime testStartDateTime = DateTime.UtcNow;
            byte collectionPeriod = 1;

            IVerificationService verificationService = autofacContainer.Resolve<IVerificationService>();
            ITestOrchestrator orchestrator = autofacContainer.Resolve<ITestOrchestrator>();
            ISubmissionService submissionService = autofacContainer.Resolve<ISubmissionService>();
            var filelist = await orchestrator.SetupTestFiles();

            // Act
            var results = await orchestrator.SubmitFiles(filelist);

            DateTime testEndDateTime = DateTime.UtcNow;
            // Assert
            results.Should().NotBeNull();

            short academicYear = (short) results.FirstOrDefault().CollectionYear;

            string csvString = await verificationService.GetVerificationDataCsv(academicYear, collectionPeriod, true,
                testStartDateTime,
                testEndDateTime);
            
             //publish the csv.
            string fileName = $"Verfication_{DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")}.csv";
            var cbs = await submissionService.GetBlobStream(fileName, academicYear == 1920 ? "ILR1920" : "ILR1819");
            byte[] buffer = Encoding.UTF8.GetBytes(csvString);
            await cbs.WriteAsync(buffer, 0, buffer.Length);
            cbs.Close();

            decimal actualPercentage = await verificationService.GetTheNumber(academicYear, collectionPeriod, true,
                testStartDateTime,
                testEndDateTime);
            decimal expected = 0.5m;

            actualPercentage.Should().BeLessOrEqualTo(expected);

            // Clean up
            await orchestrator.DeleteTestFiles(results.Select(x => x.FileName));
        }
    }
}