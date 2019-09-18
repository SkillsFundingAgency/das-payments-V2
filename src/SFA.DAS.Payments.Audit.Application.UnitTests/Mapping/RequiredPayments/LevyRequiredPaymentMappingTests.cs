using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.RequiredPayments
{
    [TestFixture]
    public class LevyRequiredPaymentMappingTests : RequiredPaymentsMappingTests<CalculatedRequiredLevyAmount>
    {
        protected override CalculatedRequiredLevyAmount CreatePaymentEvent()
        {
            return new CalculatedRequiredLevyAmount
            {
                ContractType = ContractType.Act1,
                ApprenticeshipId = 400L,
                ApprenticeshipPriceEpisodeId = 800L
            };
        }
    }
}