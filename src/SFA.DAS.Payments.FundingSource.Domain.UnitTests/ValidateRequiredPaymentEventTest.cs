using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using System.Linq;

namespace SFA.DAS.Payments.FundingSource.Domain.UnitTests
{
    [TestFixture]
    public class ValidateRequiredPaymentEventTest
    {
        [TestCase(0, RequiredPaymentEventValidationRules.ZeroSfaContributionPercentage)]
        public void SholuldGenerateValidationResultForInvalidEvent(decimal sfaContribution, int expectedValidationRule)
        {
            var message = new CoInvestedPayment
            {
                SfaContributionPercentage = sfaContribution
            };

            var validator = new ValidateRequiredPaymentEvent();
            var results = validator.Validate(message);

            Assert.IsNotNull(results);
            Assert.AreEqual(expectedValidationRule, (int)results.First().Rule);
        }

    }
}
