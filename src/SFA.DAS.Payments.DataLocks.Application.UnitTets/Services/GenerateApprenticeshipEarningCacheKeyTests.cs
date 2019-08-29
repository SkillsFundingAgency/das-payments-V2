using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using FluentAssertions;
using Moq;
using SFA.DAS.Payments.DataLocks.Application.Services;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{

    [TestFixture]
    public class GenerateApprenticeshipEarningCacheKeyTest
    {
        [Test]
        [TestCase(ApprenticeshipEarningCacheKeyTypes.Act1EarningsKey, 1, 2, "Act1EarningsKey_1_2")]
        [TestCase(ApprenticeshipEarningCacheKeyTypes.Act1FunctionalSkillEarningsKey, 1, 2, "Act1FunctionalSkillEarningsKey_1_2")]
        [TestCase(ApprenticeshipEarningCacheKeyTypes.Act1FunctionalSkillPayableEarningsKey, 1, 2, "Act1FunctionalSkillPayableEarningsKey_1_2")]
        [TestCase(ApprenticeshipEarningCacheKeyTypes.Act1PayableEarningsKey, 1, 2, "Act1PayableEarningsKey_1_2")]
        public void Generates_Key_Correctly(ApprenticeshipEarningCacheKeyTypes keyType, long ukprn, long uln, string expectedResult)
        {
            var service = new GenerateApprenticeshipEarningCacheKey();
            var actual = service.GenerateKey(keyType, ukprn, uln);
            actual.Should().Be(expectedResult);
        }
        
    }
}
