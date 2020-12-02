using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Mapping;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Mapping
{
    [TestFixture]
    public class ReportingAimFundingLineTypeMappingTest
    {
        private IMapper mapper;

        [OneTimeSetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ProviderPaymentsProfile>());
            config.AssertConfigurationIsValid();
            mapper = new Mapper(config);
        }

        [Test]
        [TestCase(ContractType.Act1, ApprenticeshipEmployerType.Levy, "16-18 Apprenticeship (From May 2017) Levy Contract", "16-18 Apprenticeship (Employer on App Service) Levy funding")]
        [TestCase(ContractType.Act1, ApprenticeshipEmployerType.NonLevy, "16-18 Apprenticeship (From May 2017) Levy Contract", "16-18 Apprenticeship (Employer on App Service) Non-Levy funding")]
        [TestCase(ContractType.Act1, ApprenticeshipEmployerType.Levy, "16-18 Apprenticeship (Employer on App Service)", "16-18 Apprenticeship (Employer on App Service) Levy funding")]
        [TestCase(ContractType.Act1, ApprenticeshipEmployerType.NonLevy, "16-18 Apprenticeship (Employer on App Service)", "16-18 Apprenticeship (Employer on App Service) Non-Levy funding")]
        [TestCase(ContractType.Act1, ApprenticeshipEmployerType.Levy, "19+ Apprenticeship (From May 2017) Levy Contract", "19+ Apprenticeship (Employer on App Service) Levy funding")]
        [TestCase(ContractType.Act1, ApprenticeshipEmployerType.NonLevy, "19+ Apprenticeship (From May 2017) Levy Contract", "19+ Apprenticeship (Employer on App Service) Non-Levy funding")]
        [TestCase(ContractType.Act1, ApprenticeshipEmployerType.Levy, "19+ Apprenticeship (Employer on App Service)", "19+ Apprenticeship (Employer on App Service) Levy funding")]
        [TestCase(ContractType.Act1, ApprenticeshipEmployerType.NonLevy, "19+ Apprenticeship (Employer on App Service)", "19+ Apprenticeship (Employer on App Service) Non-Levy funding")]

        [TestCase(ContractType.Act2, ApprenticeshipEmployerType.NonLevy, "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)", "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)")]
        [TestCase(ContractType.Act2, ApprenticeshipEmployerType.NonLevy, "16-18 Apprenticeship Non-Levy Contract (procured)", "16-18 Apprenticeship Non-Levy Contract (procured)")]
        [TestCase(ContractType.Act2, ApprenticeshipEmployerType.NonLevy, "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)", "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)")]
        [TestCase(ContractType.Act2, ApprenticeshipEmployerType.NonLevy, "19+ Apprenticeship Non-Levy Contract (procured)", "19+ Apprenticeship Non-Levy Contract (procured)")]

        [TestCase(ContractType.Act2, ApprenticeshipEmployerType.NonLevy, "Alice in wonderland", "None")]

        public void SetsReportingAimFundingLineType(ContractType contractType, ApprenticeshipEmployerType employerType, string fundingLineType, string expected)
        {
            var fundingSourceEvent = new EmployerCoInvestedFundingSourcePaymentEvent
            {
                ContractType = contractType,
                LearningAim = new LearningAim {FundingLineType = fundingLineType},
                ApprenticeshipEmployerType = employerType
            };

            var providerPayment = mapper.Map<ProviderPaymentEventModel>(fundingSourceEvent);

            providerPayment.ReportingAimFundingLineType.Should().StartWith(expected);
        }
    }
}
