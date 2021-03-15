using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.PeriodEnd.Application.Repositories;
using SFA.DAS.Payments.PeriodEnd.Application.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;

namespace SFA.DAS.Payments.PeriodEnd.Application.UnitTests.SubmissionJobsServiceTests
{
    [TestFixture]
    public class SuccessfulSubmissionsTests
    {
        private AutoMock mocker;
        private Mock<IPeriodEndRepository> mockPeriodEndRepository;
        private Mock<IProvidersRequiringReprocessingRepository> mockProvidersRequiringReprocessingRepository;
        private List<LatestSuccessfulJobModel> testJobs;
        private SubmissionJobsService sut;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mockPeriodEndRepository = mocker.Mock<IPeriodEndRepository>();
            mockProvidersRequiringReprocessingRepository = mocker.Mock<IProvidersRequiringReprocessingRepository>();
            testJobs = mocker.Create<List<LatestSuccessfulJobModel>>();
            sut = mocker.Create<SubmissionJobsService>();
        }

        [Test]
        public async Task WhenProviderHasASuccessfulJob_AndProviderNeedsReprocessing_ThenNotIncludedInResult()
        {
            //Arrange
            mockPeriodEndRepository
                .Setup(x => x.GetLatestSuccessfulJobs())
                .ReturnsAsync(testJobs);

            mockProvidersRequiringReprocessingRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync(testJobs.Select(x => x.Ukprn).ToList());

            //Act
            var result = await sut.SuccessfulSubmissions();

            //Assert
            result.SuccessfulSubmissionJobs.Should().BeEmpty();
        }

        [Test]
        public async Task WhenProviderHasASuccessfulJob_AndProviderDoesNotNeedsReprocessing_ThenIncludedInResult()
        {
            //Arrange
            mockPeriodEndRepository
                .Setup(x => x.GetLatestSuccessfulJobs())
                .ReturnsAsync(testJobs);

            mockProvidersRequiringReprocessingRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync(new List<long>());

            //Act
            var result = await sut.SuccessfulSubmissions();

            //Assert
            result.SuccessfulSubmissionJobs.Should().ContainEquivalentOf(new {Ukprn = testJobs[0].Ukprn});
        }

        [Test]
        public async Task WhenProviderNeedsReprocessing_AndDoesNotHaveASuccessfulJob_ThenNotIncludedInResult()
        {
            var testProvider = -123456789;

            //Arrange
            mockPeriodEndRepository
                .Setup(x => x.GetLatestSuccessfulJobs())
                .ReturnsAsync(testJobs);

            mockProvidersRequiringReprocessingRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync(new List<long> { testProvider, });

            //Act
            var result = await sut.SuccessfulSubmissions();

            //Assert
            result.SuccessfulSubmissionJobs.Should().NotContain(x => x.Ukprn == testProvider);
        }
    }
}