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
        private Mock<IProvidersRequiringReprocessingRepository> mockRepository;
        private List<LatestSuccessfulJobModel> testJobs;
        private ProvidersRequiringReprocessingService sut;

        private short academicYear;
        private byte collectionPeriod;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mockRepository = mocker.Mock<IProvidersRequiringReprocessingRepository>();
            testJobs = mocker.Create<List<LatestSuccessfulJobModel>>();
            sut = mocker.Create<ProvidersRequiringReprocessingService>();
            academicYear = short.MinValue;
            collectionPeriod = byte.MinValue;
        }

        [Test]
        public async Task WhenProviderHasASuccessfulJob_AndProviderNeedsReprocessing_ThenNotIncludedInResult()
        {
            //Arrange
            mockRepository
                .Setup(x => x.GetLatestSuccessfulJobs(academicYear, collectionPeriod))
                .ReturnsAsync(testJobs);

            mockRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync(testJobs.Select(x => x.Ukprn).ToList());

            //Act
            var result = await sut.SuccessfulSubmissions(academicYear, collectionPeriod);

            //Assert
            result.SuccessfulSubmissionJobs.Should().BeEmpty();
        }

        [Test]
        public async Task WhenProviderHasASuccessfulJob_AndProviderDoesNotNeedsReprocessing_ThenIncludedInResult()
        {
            //Arrange
            mockRepository
                .Setup(x => x.GetLatestSuccessfulJobs(academicYear, collectionPeriod))
                .ReturnsAsync(testJobs);

            mockRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync(new List<long>());

            //Act
            var result = await sut.SuccessfulSubmissions(academicYear, collectionPeriod);

            //Assert
            result.SuccessfulSubmissionJobs.Should().ContainEquivalentOf(new {Ukprn = testJobs[0].Ukprn});
        }

        [Test]
        public async Task WhenProviderNeedsReprocessing_AndDoesNotHaveASuccessfulJob_ThenNotIncludedInResult()
        {
            var testProvider = -123456789;

            //Arrange
            mockRepository
                .Setup(x => x.GetLatestSuccessfulJobs(academicYear, collectionPeriod))
                .ReturnsAsync(testJobs);

            mockRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync(new List<long> { testProvider, });

            //Act
            var result = await sut.SuccessfulSubmissions(academicYear, collectionPeriod);

            //Assert
            result.SuccessfulSubmissionJobs.Should().NotContain(x => x.Ukprn == testProvider);
        }
    }
}