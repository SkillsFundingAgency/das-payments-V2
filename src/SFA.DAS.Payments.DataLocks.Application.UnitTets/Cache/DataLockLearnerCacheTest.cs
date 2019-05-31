using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Cache;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Cache
{
    [TestFixture]
    public class DataLockLearnerCacheTest
    {
        private Mock<IActorDataCache<List<ApprenticeshipModel>>> dataCache;

        [Test]
        public async Task HasLearnerRecordsShouldReturnFalseIfCacheIsEmpty()
        {
            dataCache = new Mock<IActorDataCache<List<ApprenticeshipModel>>>();
            dataCache.
                Setup(o => o.IsEmpty(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            var dataLockLearnerCache = new DataLockLearnerCache(dataCache.Object);
            var actual = await dataLockLearnerCache.HasLearnerRecords().ConfigureAwait(false);
            actual.Should().BeFalse();
        }

        [Test]
        public async Task ShouldReturnApprenticeshipsIfCacheIsNotEmpty()
        {
            var apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Ukprn = 100,
                    AccountId = 100
                }
            };
            var cacheApprenticeships = new ConditionalValue<List<ApprenticeshipModel>>(true, apprenticeships);
            dataCache = new Mock<IActorDataCache<List<ApprenticeshipModel>>>();
            dataCache.Setup(o => o.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.FromResult(cacheApprenticeships));

            var dataLockLearnerCache = new DataLockLearnerCache(dataCache.Object);
            var actual = await dataLockLearnerCache.GetLearnerApprenticeships(100).ConfigureAwait(false);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);
        }

        [Test]
        public async Task ShouldReturnDuplicateApprenticeshipsIfCacheIsNotEmpty()
        {
            var duplicateApprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Ukprn = 100,
                    AccountId = 200
                }
            };
            var cacheApprenticeships = new ConditionalValue<List<ApprenticeshipModel>>(true, duplicateApprenticeships);
            dataCache = new Mock<IActorDataCache<List<ApprenticeshipModel>>>();
            dataCache.Setup(o => o.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(cacheApprenticeships));

            var dataLockLearnerCache = new DataLockLearnerCache(dataCache.Object);
            var actual = await dataLockLearnerCache.GetDuplicateApprenticeships().ConfigureAwait(false);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);
        }

    }
}
