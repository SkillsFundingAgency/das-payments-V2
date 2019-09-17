using System;
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
        public async Task SmokeTest()
        {
            // Arrange
            DateTimeOffset testStartDateTime = DateTimeOffset.UtcNow;
            var startString = $"StartTime Value: {testStartDateTime:O}";
            TestContext.WriteLine(startString);

            ITestOrchestrator orchestrator = autofacContainer.Resolve<ITestOrchestrator>();
            var fileList = await orchestrator.SetupTestFiles();

            // Act
            var results = await orchestrator.SubmitFiles(fileList);
            var resultsList = results.ToList();

            var configuration = autofacContainer.Resolve<Configuration>();
            DateTimeOffset testEndDateTime = DateTimeOffset.UtcNow;
            DateTime maxWaitDateTime = DateTime.UtcNow.Add(configuration.MaxTimeout);
            while (true)
            {
                if (DateTime.UtcNow >= maxWaitDateTime)
                    break;

                await Task.Delay(15000);
                DateTimeOffset? newDateTime =
                    await orchestrator.GetNewDateTime(resultsList.Select(r => r.Ukprn).ToList());

                if (!newDateTime.HasValue)
                {
                    break;
                }

                if (newDateTime.Value > testEndDateTime)
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

            // Assert
            resultsList.All(x => x.Status == JobStatusType.Completed).Should().BeTrue();


            await orchestrator.VerifyResults(resultsList, testStartDateTime, testEndDateTime,
                (actualPercentage, tolerance, earningDifference) =>
                {
                    TestContext.WriteLine($"Earning difference between DC and DAS: {earningDifference}");
                    earningDifference.Should().Be(0m);

                    if (!actualPercentage.HasValue)
                    {
                        var nullPercentageMessage = "The returned percentage was null";
                        TestContext.WriteLine(nullPercentageMessage);
                        Assert.Inconclusive(nullPercentageMessage);
                    }
                    else
                    {
                        TestContext.WriteLine($"Returned Percentage: {actualPercentage.Value}");
                        actualPercentage.Should().BeLessOrEqualTo(tolerance);
                    }
                });
        }
    }
}