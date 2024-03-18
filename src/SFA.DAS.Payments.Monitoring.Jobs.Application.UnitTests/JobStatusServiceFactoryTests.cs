using System;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.Earnings;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]
    public class JobStatusServiceFactoryTests
    {
        private AutoMock mocker;
        private Mock<IUnitOfWorkScope> mockScope;
        private JobStatusServiceFactory factory;
        [SetUp]
        public void Setup()
        {
            mocker = AutoMock.GetLoose();
            mockScope = mocker.Mock<IUnitOfWorkScope>();
            factory = new JobStatusServiceFactory();
            mockScope.Setup(scope => scope.Resolve<IEarningsJobStatusService>()).Returns(Mock.Of<IEarningsJobStatusService>());
            mockScope.Setup(scope => scope.Resolve<IPeriodEndStartJobStatusService>()).Returns(Mock.Of<IPeriodEndStartJobStatusService>());
            mockScope.Setup(scope => scope.Resolve<IIlrReprocessingJobStatusService>()).Returns(Mock.Of<IIlrReprocessingJobStatusService>());
            mockScope.Setup(scope => scope.Resolve<IPeriodEndJobStatusService>()).Returns(Mock.Of<IPeriodEndJobStatusService>());
        }

        [Test]
        public void Returns_EarningsJobService_For_EarningsJob_Type()
        {
            var service = factory.Create(mockScope.Object, JobType.EarningsJob);
            mockScope.Verify(scope => scope.Resolve<IEarningsJobStatusService>());
            Assert.IsInstanceOf<IEarningsJobStatusService>(service);
        }

        [TestCase(typeof(IEarningsJobStatusService),JobType.EarningsJob)]
        [TestCase(typeof(IEarningsJobStatusService), JobType.ComponentAcceptanceTestEarningsJob)]
        [TestCase(typeof(IPeriodEndStartJobStatusService), JobType.PeriodEndStartJob)]
        [TestCase(typeof(IIlrReprocessingJobStatusService), JobType.PeriodEndIlrReprocessingJob)]
        [TestCase(typeof(IPeriodEndJobStatusService), JobType.PeriodEndRunJob)]
        [TestCase(typeof(IPeriodEndJobStatusService), JobType.ComponentAcceptanceTestMonthEndJob)]
        [TestCase(typeof(IPeriodEndJobStatusService), JobType.PeriodEndStopJob)]
        public void Returns_Correct_Job_Status_Service_For_Job_Type(Type jobServiceType, JobType jobType)
        {
            var service = factory.Create(mockScope.Object, jobType);
            Assert.IsInstanceOf(jobServiceType, service);
        }

        [TestCase(JobType.PeriodEndSubmissionWindowValidationJob)]
        [TestCase(JobType.PeriodEndRequestReportsJob)]
        public void Throws_Exception_For_Jobs_Not_Handled_Using_A_JobStatusService(JobType jobType)
        {
            Assert.Throws<InvalidOperationException>(() => factory.Create(mockScope.Object, jobType));
        }
    }
}