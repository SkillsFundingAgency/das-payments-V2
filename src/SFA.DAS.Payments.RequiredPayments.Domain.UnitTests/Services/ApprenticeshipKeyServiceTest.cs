using System;
using NUnit.Framework;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class ApprenticeshipKeyServiceTest
    {
        [Test]
        public void ApprenticeshipKeyContainsAllElements()
        {
            // arrange
            var learnerReferenceNumber = "1";
            var ukprn = 2;
            var frameworkCode = 3;
            var pathwayCode = 4;
            var programmeType = 25;
            var standardCode = 5;
            var learnAimRef = "6";

            // act
            var key = new ApprenticeshipKeyService().GenerateApprenticeshipKey(ukprn, learnerReferenceNumber, frameworkCode, pathwayCode, programmeType, standardCode, learnAimRef);

            // assert
            Assert.AreEqual(0, key.IndexOf("2", StringComparison.Ordinal), "UKPRN should go first");
            Assert.Less(key.IndexOf("2", StringComparison.Ordinal), key.IndexOf("1", StringComparison.Ordinal), "LearnRefNumber should be after UKPRN");
            Assert.Less(key.IndexOf("1", StringComparison.Ordinal), key.IndexOf("3", StringComparison.Ordinal), "FrameworkCode should be after LearnRefNumber");
            Assert.Less(key.IndexOf("3", StringComparison.Ordinal), key.IndexOf("4", StringComparison.Ordinal), "PathwayCode should be after FrameworkCode");
            Assert.Less(key.IndexOf("4", StringComparison.Ordinal), key.IndexOf("25", StringComparison.Ordinal), "ProgrammeType should be after PathwayCode");
            Assert.Less(key.IndexOf("25", StringComparison.Ordinal), key.IndexOf("5", StringComparison.Ordinal), "StandardCode should be after ProgrammeType");
            Assert.Less(key.IndexOf("5", StringComparison.Ordinal), key.IndexOf("6", StringComparison.Ordinal), "LearnAimRef should be after StandardCode");
        }


        [Test]
        public void TestApprenticeshipKeyChangesCase()
        {
            // arrange
            var learnerReferenceNumber = "A";
            var ukprn = 2;
            var frameworkCode = 3;
            var pathwayCode = 4;
            var programmeType = 25;
            var standardCode = 5;
            var learnAimRef = "B";

            // act
            var key = new ApprenticeshipKeyService().GenerateApprenticeshipKey(ukprn, learnerReferenceNumber, frameworkCode, pathwayCode, programmeType, standardCode, learnAimRef);

            // assert
            Assert.IsFalse(key.Contains("A"));
            Assert.IsFalse(key.Contains("B"));
            Assert.IsTrue(key.Contains("a"));
            Assert.IsTrue(key.Contains("b"));
        }


        [Test]
        public void CanParseValidApprenticeshipKey()
        {
            // arrange
            var learnerReferenceNumber = "1";
            var ukprn = 2;
            var frameworkCode = 3;
            var pathwayCode = 4;
            var programmeType = 25;
            var standardCode = 5;
            var learnAimRef = "6";
            var service = new ApprenticeshipKeyService();
            var key = service.GenerateApprenticeshipKey(ukprn, learnerReferenceNumber, frameworkCode, pathwayCode, programmeType, standardCode, learnAimRef);

            // act
            var apprenticeshipKey = service.ParseApprenticeshipKey(key);

            // assert
            Assert.AreEqual(apprenticeshipKey.Ukprn, ukprn);
            Assert.AreEqual(apprenticeshipKey.LearnerReferenceNumber, learnerReferenceNumber);
            Assert.AreEqual(apprenticeshipKey.FrameworkCode, frameworkCode);
            Assert.AreEqual(apprenticeshipKey.PathwayCode, pathwayCode);
            Assert.AreEqual(apprenticeshipKey.ProgrammeType, programmeType);
            Assert.AreEqual(apprenticeshipKey.StandardCode, standardCode);
            Assert.AreEqual(apprenticeshipKey.LearnAimRef, learnAimRef);
        }

        [Test]
        public void CanParseApprenticeshipKey_With_Null_Values()
        {
            // arrange
            var learnerReferenceNumber = "1";
            var ukprn = 2;
            var frameworkCode = 3;
            var pathwayCode = 4;
            var programmeType = 25;
            var standardCode = 5;
            string learnAimRef = null;
            var service = new ApprenticeshipKeyService();
            var key = service.GenerateApprenticeshipKey(ukprn, learnerReferenceNumber, frameworkCode, pathwayCode, programmeType, standardCode, learnAimRef);

            // act
            var apprenticeshipKey = service.ParseApprenticeshipKey(key);

            // assert
            Assert.AreEqual(apprenticeshipKey.Ukprn, ukprn);
            Assert.AreEqual(apprenticeshipKey.LearnerReferenceNumber, learnerReferenceNumber);
            Assert.AreEqual(apprenticeshipKey.FrameworkCode, frameworkCode);
            Assert.AreEqual(apprenticeshipKey.PathwayCode, pathwayCode);
            Assert.AreEqual(apprenticeshipKey.ProgrammeType, programmeType);
            Assert.AreEqual(apprenticeshipKey.StandardCode, standardCode);
            Assert.AreEqual(apprenticeshipKey.LearnAimRef, string.Empty);
        }
    }
}
