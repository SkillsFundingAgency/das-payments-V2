using System;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.PeriodEnd.Application.Repositories;
using SFA.DAS.Payments.PeriodEnd.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;

namespace SFA.DAS.Payments.PeriodEnd.Application.UnitTests.SubmissionJobsServiceTests
{
    [TestFixture]
    public class SuccessfulSubmissionsTests
    {
        private AutoMock mocker;
        private Mock<IPeriodEndRepository> mockPeriodEndRepository;
        private List<LatestSuccessfulJobModel> testJobs;
        private SubmissionJobsService sut;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mockPeriodEndRepository = mocker.Mock<IPeriodEndRepository>();
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

            //Act
            var result = await sut.SuccessfulSubmissions();

            //Assert
            
        }

        [Test]
        public async Task WhenProviderHasASuccessfulJob_AndProviderDoesNotNeedsReprocessing_ThenIncludedInResult()
        {
        }
    }
}