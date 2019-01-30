using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.EarningEvent
{
    public class ApprenticeshipContractType1MappingTests : ApprenticeshipContractTypeMappingTests<ApprenticeshipContractType1EarningEvent>
    {
        public ApprenticeshipContractType1MappingTests() : base(ContractType.Act1)
        {


        }

        [Test]
        public void Maps_AgreementId()
        {
            PaymentEvent.AgreementId = "AGREEMENTID";

            var model = Mapper.Map<EarningEventModel>(PaymentEvent);
            model.AgreementId.Should().Be(PaymentEvent.AgreementId);
        }
    }
}