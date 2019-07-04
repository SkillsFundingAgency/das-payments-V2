using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    [TestFixture()]
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
                    ApprenticeshipId = 1,
                    ApprenticeshipPriceEpisodeIds = new List<long> { 1,3,2 }
                },
                new DataLockFailure
                {
                    DataLockError = DataLockErrorCode.DLOCK_04,
                    ApprenticeshipId = 2,
                    ApprenticeshipPriceEpisodeIds = new List<long> { 3,2,1 }
                }
            };

            var newFailures = new List<DataLockFailure>
            {
                new DataLockFailure
                {
                    DataLockError = DataLockErrorCode.DLOCK_04,
                    ApprenticeshipId = 2,
                    ApprenticeshipPriceEpisodeIds = new List<long> {3, 2, 1}
                },
                new DataLockFailure
                {
                    DataLockError = DataLockErrorCode.DLOCK_03,
                    ApprenticeshipId = 1,
                    ApprenticeshipPriceEpisodeIds = new List<long> {2, 3, 1}
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
