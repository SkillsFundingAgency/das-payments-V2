using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.LearnerMatching
{
    [TestFixture]
    public class UkprnMatcherTests
    {
        [Test]
        public void Should_Return_DataLock_01_When_No_Apprenticeships_Match_Ukprn()
        {
            var apprenticeships = new List<ApprenticeshipModel> { new ApprenticeshipModel { Ukprn = 123456 } };

            var ukprnMatcher = new UkprnMatcher();

            var result = ukprnMatcher.MatchUkprn(12345, apprenticeships);

            result.Should().NotBeNull();
            result.DataLockErrorCode.Should().NotBeNull();
            result.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_01);
        }

        [Test]
        public void Should_Return_DataLock_01_When_No_Apprenticeships()
        {
            var apprenticeships = new List<ApprenticeshipModel>();

            var ukprnMatcher = new UkprnMatcher();

            var result = ukprnMatcher.MatchUkprn(12345, apprenticeships);

            result.Should().NotBeNull();
            result.DataLockErrorCode.Should().NotBeNull();
            result.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_01);
        }

        [Test]
        public void Should_Not_Return_DataLock_01_When_Apprenticeships_Match_Ukprn()
        {
            var apprenticeships = new List<ApprenticeshipModel> { new ApprenticeshipModel { Ukprn = 12345 } };

            var ukprnMatcher = new UkprnMatcher();

            var result = ukprnMatcher.MatchUkprn(12345, apprenticeships);

            result.Should().NotBeNull();
            result.DataLockErrorCode.Should().BeNull();
        }

        [Test]
        public void Should_Return_Matched_Apprenticeships()
        {
            var apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel { Ukprn = 12345, Uln = 1 },
                new ApprenticeshipModel { Ukprn = 12345, Uln = 2 },
                new ApprenticeshipModel { Ukprn = 12345, Uln = 3 },
                new ApprenticeshipModel { Ukprn = 123456, Uln = 4 }
            };

            var ukprnMatcher = new UkprnMatcher();

            var result = ukprnMatcher.MatchUkprn(12345, apprenticeships);

            result.Should().NotBeNull();
            result.Apprenticeships.Count(a => a.Uln == 1).Should().Be(1);
            result.Apprenticeships.Count(a => a.Uln == 2).Should().Be(1);
            result.Apprenticeships.Count(a => a.Uln == 3).Should().Be(1);
            result.Apprenticeships.Count(a => a.Uln == 4).Should().Be(0);
        }
    }
}
