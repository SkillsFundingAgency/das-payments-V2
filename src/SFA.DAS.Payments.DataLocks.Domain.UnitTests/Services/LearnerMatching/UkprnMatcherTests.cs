using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.LearnerMatching
{
    [TestFixture]
    public class UkprnMatcherTests
    {
        private Mock<IDataLockLearnerCache> dataLockLearnerCache;

        [Test]
        public async Task ShouldReturnDataLockOneWhenCacheIsEmpty()
        {
            dataLockLearnerCache = new Mock<IDataLockLearnerCache>();
            dataLockLearnerCache
                .Setup(o => o.UkprnExists(It.IsAny<long>()))
                .Returns(Task.FromResult(false));

            var ukprnMatcher = new UkprnMatcher(dataLockLearnerCache.Object);

            var result = await ukprnMatcher.MatchUkprn(12345).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Should().Be(DataLockErrorCode.DLOCK_01);
        }

        [Test]
        public async Task ShouldReturnNullWhenCacheHasLearnerRecords()
        {
            dataLockLearnerCache = new Mock<IDataLockLearnerCache>();
            dataLockLearnerCache
                .Setup(o => o.UkprnExists(It.IsAny<long>()))
                .Returns(Task.FromResult(true));

            var ukprnMatcher = new UkprnMatcher(dataLockLearnerCache.Object);
            var result = await ukprnMatcher.MatchUkprn(1234).ConfigureAwait(false);
            result.Should().BeNull();
        }
    }
}
