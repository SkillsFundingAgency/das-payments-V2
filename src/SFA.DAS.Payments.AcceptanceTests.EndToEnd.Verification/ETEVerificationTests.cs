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
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using FluentAssertions;
using NUnit.Framework;
using Polly;
using Polly.Registry;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
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

            builder.RegisterType<Configuration>().SingleInstance();
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

            builder.Register((c, p) =>
                             {
                                 var configHelper = c.Resolve<Configuration>();
                                 return new TestPaymentsDataContext(configHelper.PaymentsConnectionString);
                             }).As<TestPaymentsDataContext>().InstancePerLifetimeScope();
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
            Console.WriteLine($"StartTime Value: {testStartDateTime}");

            IVerificationService verificationService = autofacContainer.Resolve<IVerificationService>();
            ITestOrchestrator orchestrator = autofacContainer.Resolve<ITestOrchestrator>();
            ISubmissionService submissionService = autofacContainer.Resolve<ISubmissionService>();
            var filelist = await orchestrator.SetupTestFiles();

            // Act
            var results = await orchestrator.SubmitFiles(filelist);
            DateTime testEndDateTime = DateTime.UtcNow;
            Console.WriteLine($"EndTime Value: {testEndDateTime}");

            results.Should().NotBeNullOrEmpty();
           
            // Assert
            results.All(x => x.Status == JobStatusType.Completed).Should().BeTrue();

            byte collectionPeriod = (byte)results.FirstOrDefault().PeriodNumber;

            var groupedResults = results.ToList().GroupBy(g => g.CollectionYear);

            foreach (var groupedResult in groupedResults)
            {
                short academicYear = (short) groupedResult.Key;

                string csvString = await verificationService.GetVerificationDataCsv(academicYear, collectionPeriod,
                    true,
                    testStartDateTime,
                    testEndDateTime);

                //publish the csv.
                await FileHelpers.UploadCsvFile(FileHelpers.ReportType.PaymentsData, academicYear, collectionPeriod, submissionService, csvString);

                var secondDataCsv = await verificationService.GetDataStoreCsv(academicYear, collectionPeriod);

                //publish the csv.
                await FileHelpers.UploadCsvFile(FileHelpers.ReportType.DataStore, academicYear, collectionPeriod, submissionService, secondDataCsv);

                decimal actualPercentage = await verificationService.GetTheNumber(academicYear, collectionPeriod, true,
                    testStartDateTime,
                    testEndDateTime);
                decimal expected = 0.5m;

                actualPercentage.Should().BeLessOrEqualTo(expected);
            }
        }
    }
}