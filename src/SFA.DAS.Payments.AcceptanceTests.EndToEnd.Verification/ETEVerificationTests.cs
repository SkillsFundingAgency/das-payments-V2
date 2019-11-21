using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using FluentAssertions;
using FluentAssertions.Execution;
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
        public async Task DBTest()
        {
            ITestOrchestrator orchestrator = autofacContainer.Resolve<ITestOrchestrator>();
            var fileUploadJobs = new List<FileUploadJob>
            {
                new FileUploadJob() {Ukprn = 10063506, PeriodNumber = 3, CollectionYear = 1920, DateTimeSubmittedUtc = DateTime.UtcNow}, new FileUploadJob() {Ukprn = 662745, PeriodNumber = 3, CollectionYear = 1920, DateTimeSubmittedUtc = DateTime.UtcNow}
            }; 
            await orchestrator.VerifyResults(fileUploadJobs, (arg1, arg2, arg3) => { });

        }


        [Test]
        public async Task SmokeTest()
        {
            // Arrange
            ITestOrchestrator orchestrator = autofacContainer.Resolve<ITestOrchestrator>();
            var fileList = await orchestrator.SetupTestFiles();

            // Act
            var results = await orchestrator.SubmitFiles(fileList);
            var resultsList = results.ToList();

            // Assert
            using (new AssertionScope())
            {
                await orchestrator.VerifyResults(resultsList,(actualPercentage, tolerance, earningDifference) =>
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

                resultsList.Should().OnlyContain(x => x.Status == JobStatusType.Completed, "because all jobs should have completed.");
            }
        }
    }
}