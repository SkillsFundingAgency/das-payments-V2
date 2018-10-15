using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Global;

namespace SFA.DAS.Payments.EarningEvents.Domain.UnitTests.Validation.Global
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class FM36GlobalValidationRuleTests
    {
        [Test]
        public void Fails_Validation_If_Ukprn_Is_Not_Populated()
        {
            var global = new FM36Global { UKPRN = 0, Year = "1718" };
            var result = new FM36GlobalValidationRule().IsValid(global);
            Assert.IsFalse(result.Failed);
        }

        [Test]
        public void Fails_Validation_If_Collection_Year_Is_Not_Populated()
        {
            var global = new FM36Global { UKPRN = 12345, Year = null };
            var result = new FM36GlobalValidationRule().IsValid(global);
            Assert.IsFalse(result.Failed);
        }

        [Test]
        public void Passes_Validation_If_Ukprn_And_Collection_Year_Are_Valid()
        {
            var global = new FM36Global { UKPRN = 12345, Year = "1718" };
            var result = new FM36GlobalValidationRule().IsValid(global);
            Assert.IsTrue(result.Failed);
        }
    }
}