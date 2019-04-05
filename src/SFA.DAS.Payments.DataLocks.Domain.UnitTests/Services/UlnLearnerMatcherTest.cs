using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    [TestFixture]
    public class UlnLearnerMatcherTest
    {
        private Mock<IDataLockLearnerCache> dataLockLearnerCache;

        [Test]
        public async Task ShouldReturnDatalockErrorIfNoApprenticeshipExistForUln()
        {
            dataLockLearnerCache = new Mock<IDataLockLearnerCache>();
            dataLockLearnerCache
                .Setup(o => o.GetLearnerApprenticeships(It.IsAny<long>()))
                .Returns(Task.FromResult(new List<ApprenticeshipModel>()));

            var ulnLearnerMatcher = new UlnLearnerMatcher(dataLockLearnerCache.Object);
            var matchResult = await ulnLearnerMatcher.MatchUln(1).ConfigureAwait(false);

            matchResult.Should().NotBeNull();
            matchResult.ErrorCode.Should().Be(DataLockErrorCode.DLOCK_02);
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
                .Setup(o => o.GetLearnerApprenticeships(It.IsAny<long>()))
                .Returns(Task.FromResult(apprenticeships));

            var ulnLearnerMatcher = new UlnLearnerMatcher(dataLockLearnerCache.Object);
            var matchResult = await ulnLearnerMatcher.MatchUln(1).ConfigureAwait(false);

            matchResult.Should().NotBeNull();
            matchResult.Apprenticeships.Should().NotBeNull();
            matchResult.Apprenticeships.Should().HaveCount(1);
            matchResult.ErrorCode.Should().BeNull();
        }


    }
}
