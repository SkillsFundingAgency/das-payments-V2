using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.Jobs.Model.Enums;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure;

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
            builder.RegisterType<TestOrchestrator>().As<ITestOrchestrator>().InstancePerLifetimeScope();
            builder.RegisterType<VerificationService>().As<IVerificationService>().InstancePerLifetimeScope();
            builder.RegisterType<SubmissionService>().As<ISubmissionService>().InstancePerLifetimeScope();

            builder.RegisterModule<AcceptanceTestsModule>();

            autofacContainer = builder.Build();
        }

        [Test]
        public async Task InitialTest()
        {
            // Arrange
            DateTime testStartDateTime = DateTime.UtcNow;
            var startString = $"StartTime Value: {testStartDateTime:O}";
            TestContext.WriteLine(startString);

            ITestOrchestrator orchestrator = autofacContainer.Resolve<ITestOrchestrator>();
            var fileList = await orchestrator.SetupTestFiles();

            // Act
            var results = await orchestrator.SubmitFiles(fileList);


            DateTimeOffset testEndDateTime = DateTime.UtcNow;
            DateTime maxWaitDateTime = DateTime.UtcNow.AddMinutes(15);
            while (true)
            {
                if (DateTime.UtcNow >= maxWaitDateTime)
                    break;

                await Task.Delay(15000);
                DateTimeOffset? newDateTime =  await GetNewDateTime(results.Select(r=> r.Ukprn).ToList());

                if (!newDateTime.HasValue)
                {
                    break;
                }

                if (newDateTime > testEndDateTime)
                {
                    testEndDateTime = newDateTime.Value;
                }
                else
                {
                    break;
                }
            }


            var endString = $"EndTime Value: {testEndDateTime:O}";
            TestContext.WriteLine(endString);

            results.Should().NotBeNullOrEmpty();
           
            // Assert
            results.All(x => x.Status == JobStatusType.Completed).Should().BeTrue();


           await orchestrator.VerifyResults(results, testStartDateTime, testEndDateTime.DateTime, actualPercentage =>
           {
               if (!actualPercentage.HasValue)
               {
                   var nullPercentageMessage = "The returned percentage was null";
                   TestContext.WriteLine(nullPercentageMessage);
                   Assert.Inconclusive(nullPercentageMessage);
               }
               else
               {
                   decimal expected = 0.5m;
                   var returnedPercentage = $"Returned Percentage: {actualPercentage.Value}";
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