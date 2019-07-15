using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    [TestFixture]
    public class DataLockStatusServiceTest
    {
        private DataLockStatusService service;

        [SetUp]
        public void SetUp()
        {
            service = new DataLockStatusService();
        }

        [Test]
        public void TestReturnsChangeToFailedWhenNoExisting()
        {
            // arrange
            var newFailure = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_01}};

            // act
            var result = service.GetStatusChange(null, newFailure);

            // assert
            result.Should().Be(DataLockStatusChange.ChangedToFailed);
        }

        [Test]
        public void TestReturnsChangeToPassedWhenNoNew()
        {
            // arrange
            var currentFailure = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_01}};

            // act
            var result = service.GetStatusChange(currentFailure, null);

            // assert
            result.Should().Be(DataLockStatusChange.ChangedToPassed);
        }

        [Test]
        public void TestReturnsNoChangeWhenNothingChanges()
        {
            // arrange
            var existingFailure = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_01}};
            var newFailure = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_01}};

            // act
            var result = service.GetStatusChange(existingFailure, newFailure);

            // assert
            result.Should().Be(DataLockStatusChange.NoChange);
        }

        [Test]
        public void TestReturnsNoChangeWhenNothingChangesWithUnorderedValues()
        {
            // arrange
            var oldFailures = new List<DataLockFailure>
            {
                new DataLockFailure
                {
                    DataLockError = DataLockErrorCode.DLOCK_03,
                    Apprenticeship = new ApprenticeshipModel { Id = 1 },
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel {Id = 1 },
                        new ApprenticeshipPriceEpisodeModel {Id = 3 },
                        new ApprenticeshipPriceEpisodeModel {Id = 2 },
                    }
                },
                new DataLockFailure
                {
                    DataLockError = DataLockErrorCode.DLOCK_04,
                    Apprenticeship = new ApprenticeshipModel { Id = 2 },
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel {Id = 3 },
                        new ApprenticeshipPriceEpisodeModel {Id = 2 },
                        new ApprenticeshipPriceEpisodeModel {Id = 1 },
                    }
                }
            };

            var newFailures = new List<DataLockFailure>
            {
                new DataLockFailure
                {
                    DataLockError = DataLockErrorCode.DLOCK_04,
                    Apprenticeship = new ApprenticeshipModel { Id = 2 },
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel {Id = 3 },
                        new ApprenticeshipPriceEpisodeModel {Id = 2 },
                        new ApprenticeshipPriceEpisodeModel {Id = 1 },
                    }
                },
                new DataLockFailure
                {
                    DataLockError = DataLockErrorCode.DLOCK_03,
                    Apprenticeship = new ApprenticeshipModel { Id = 1},
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel {Id = 2 },
                        new ApprenticeshipPriceEpisodeModel {Id = 3 },
                        new ApprenticeshipPriceEpisodeModel {Id = 1 },
                    }
                }
            };

            // act
            var result = service.GetStatusChange(oldFailures, newFailures);

            // assert
            result.Should().Be(DataLockStatusChange.NoChange);
        }

        [Test]
        public void TestReturnsNoChangeWhenNoFailures()
        {
            // arrange
            // act
            var result = service.GetStatusChange(null, null);

            // assert
            result.Should().Be(DataLockStatusChange.NoChange);
        }
    }
}
