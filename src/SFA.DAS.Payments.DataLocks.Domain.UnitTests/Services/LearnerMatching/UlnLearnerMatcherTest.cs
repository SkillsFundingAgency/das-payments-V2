using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.LearnerMatching
{
    [TestFixture]
    public class UlnLearnerMatcherTest
    {
        private Mock<IDataLockLearnerCache> dataLockLearnerCache;
        private long uln = 1;

        [Test]
        public async Task ShouldReturnDatalockErrorIfNoApprenticeshipExistForUln()
        {
            dataLockLearnerCache = new Mock<IDataLockLearnerCache>();
            dataLockLearnerCache
                .Setup(o => o.GetLearnerApprenticeships(uln))
                .Returns(Task.FromResult(new List<ApprenticeshipModel>()));

            var ulnLearnerMatcher = new UlnLearnerMatcher(dataLockLearnerCache.Object);
            var matchResult = await ulnLearnerMatcher.MatchUln(uln).ConfigureAwait(false);

            matchResult.Should().NotBeNull();
            matchResult.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_02);
            matchResult.Apprenticeships.Should().BeNull();
        }

        [Test]
        public async Task ShouldReturnApprenticeshipsForUlnIfRecordExist()
        {
            var apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel()
            };

            dataLockLearnerCache = new Mock<IDataLockLearnerCache>();
            dataLockLearnerCache
                .Setup(o => o.GetLearnerApprenticeships(uln))
                .Returns(Task.FromResult(apprenticeships));

            var ulnLearnerMatcher = new UlnLearnerMatcher(dataLockLearnerCache.Object);
            var matchResult = await ulnLearnerMatcher.MatchUln(uln).ConfigureAwait(false);

            matchResult.Should().NotBeNull();
            matchResult.Apprenticeships.Should().NotBeNull();
            matchResult.Apprenticeships.Should().HaveCount(1);
            matchResult.DataLockErrorCode.Should().BeNull();
        }


    }
}
