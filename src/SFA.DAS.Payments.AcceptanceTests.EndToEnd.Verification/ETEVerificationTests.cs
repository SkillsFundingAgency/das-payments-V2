using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using FluentAssertions;
using NUnit.Framework;
using Polly;
using Polly.Registry;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
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
            var startString = $"StartTime Value: {testStartDateTime}";
            Console.WriteLine(startString);
            TestContext.WriteLine(startString);

            ITestOrchestrator orchestrator = autofacContainer.Resolve<ITestOrchestrator>();
            var filelist = await orchestrator.SetupTestFiles();

            // Act
            var results = await orchestrator.SubmitFiles(filelist);


            DateTime testEndDateTime = DateTime.UtcNow;

            while (true)
            {
                await Task.Delay(1000);
                DateTimeOffset? newDateTime =  await GetNewDateTime(results.Select(r=> r.Ukprn).ToList());

                if (!newDateTime.HasValue)
                {
                    break;
                }

                if (newDateTime >= testEndDateTime)
                {
                    testEndDateTime = newDateTime.Value.DateTime;
                }
                else
                {
                    break;
                }
            }


            var endString = $"EndTime Value: {testEndDateTime}";
            Console.WriteLine(endString);
            TestContext.WriteLine(endString);

            results.Should().NotBeNullOrEmpty();
           
            // Assert
            results.All(x => x.Status == JobStatusType.Completed).Should().BeTrue();


           await orchestrator.VerifyResults(results, testStartDateTime, testEndDateTime, actualPercentage =>
           {
               if (!actualPercentage.HasValue)
               {
                   var nullPercentageMessage = "The returned percentage was null";
                   Console.WriteLine(nullPercentageMessage);
                   TestContext.WriteLine(nullPercentageMessage);
                   Assert.Inconclusive(nullPercentageMessage);
               }
               else
               {
                   decimal expected = 0.5m;
                   var returnedPercentage = $"Returned Percentage: {actualPercentage.Value}";
                   Console.WriteLine(returnedPercentage);
                   TestContext.WriteLine(returnedPercentage);
                   actualPercentage.Should().BeLessOrEqualTo(expected);
               }
           });

        }

        private async Task<DateTimeOffset?> GetNewDateTime(List<long> ukprns)
        {
            IVerificationService verificationService = autofacContainer.Resolve<IVerificationService>();

            return (await verificationService.GetLastActivityDate(ukprns));
        }
    }
}