using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.PaymentsDue
{
    public class ApprenticeshipContractType2PaymentsDueMappingTests : PaymentsDueMappingTests<ApprenticeshipContractType2PaymentDueEvent>
    {
        protected override ApprenticeshipContractType2PaymentDueEvent CreatePaymentEvent()
        {
            return new ApprenticeshipContractType2PaymentDueEvent
            {
                SfaContributionPercentage = .9M
            };
        }

        [TestCaseSource(nameof(GetOnProgrammeEarningTypes))]
        public void Maps_OnProgrammeEarningTypes(OnProgrammeEarningType earningType)
        {
            PaymentEvent.Type = earningType;
            var model = Mapper.Map<PaymentsDueEventModel>(PaymentEvent);
            model.TransactionType.Should().Be((TransactionType)PaymentEvent.Type);
        }

        public static Array GetOnProgrammeEarningTypes()
        {
            return GetEnumValues<OnProgrammeEarningType>();
        }

        public void Maps_ContractType()
        {
            Mapper.Map<PaymentsDueEventModel>(PaymentEvent).ContractType.Should().Be(ContractType.Act2);
        }

        [Test]
        public void Maps_SfaContributionPercentage()
        {
            Mapper.Map<PaymentsDueEventModel>(PaymentEvent).SfaContributionPercentage.Should().Be(PaymentEvent.SfaContributionPercentage);
        }
    }
}