using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SFA.DAS.Payments.PaymentsDue.Domain.Entities;
using SFA.DAS.Payments.PaymentsDue.Domain.Enums;

namespace SFA.DAS.Payments.PaymentsDue.UnitTests.Domain
{
    [TestFixture]
    public class ApprenticeshipTest
    {
        [Test]
        public void ApprenticeshipKeyContainsAllElements()
        {
            // arrange
            var apprenticeship = new Apprenticeship
            {
                Learner = new Learner
                {
                    LearnerReferenceNumber = "1",
                    Ukprn = 2
                },
                Course = new Course
                {
                    FrameworkCode = 3,
                    PathwayCode = 4,
                    ProgrammeType = ProgrammeType.None,
                    StandardCode = 5,
                    LearnAimRef = 6
                },
                Ukprn = 2
            };

            // act
            var key = apprenticeship.Key;

            // assert
            Assert.AreEqual(0, key.IndexOf("2", StringComparison.Ordinal), "UKPRN should go first");
            Assert.Less(key.IndexOf("2", StringComparison.Ordinal), key.IndexOf("1", StringComparison.Ordinal), "LearnRefNumber should be after UKPRN");
            Assert.Less(key.IndexOf("1", StringComparison.Ordinal), key.IndexOf("3", StringComparison.Ordinal), "FrameworkCode should be after LearnRefNumber");
            Assert.Less(key.IndexOf("3", StringComparison.Ordinal), key.IndexOf("4", StringComparison.Ordinal), "PathwayCode should be after FrameworkCode");
            Assert.Less(key.IndexOf("4", StringComparison.Ordinal), key.IndexOf("0", StringComparison.Ordinal), "ProgrammeType should be after PathwayCode");
            Assert.Less(key.IndexOf("0", StringComparison.Ordinal), key.IndexOf("5", StringComparison.Ordinal), "StandardCode should be after ProgrammeType");
            Assert.Less(key.IndexOf("5", StringComparison.Ordinal), key.IndexOf("6", StringComparison.Ordinal), "LearnAimRef should be after StandardCode");
        }
    }
}
