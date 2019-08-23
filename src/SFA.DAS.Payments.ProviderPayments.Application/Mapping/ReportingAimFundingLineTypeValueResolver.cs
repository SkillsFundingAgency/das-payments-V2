using AutoMapper;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Mapping
{
    public class ReportingAimFundingLineTypeValueResolver : IValueResolver<FundingSourcePaymentEvent, ProviderPaymentEventModel, string>
    {
        public string Resolve(FundingSourcePaymentEvent source, ProviderPaymentEventModel destination, string destMember, ResolutionContext context)
        {
            var contractType = source.ContractType;
            var employerType = source.ApprenticeshipEmployerType;
            var fundingLineType = source.LearningAim.FundingLineType;

            if (employerType != ApprenticeshipEmployerType.NonLevy && employerType != ApprenticeshipEmployerType.Levy)
                return $"None (unknown employer type {employerType})";

            if (contractType == ContractType.Act1)
            {
                switch (fundingLineType)
                {
                    case "16-18 Apprenticeship (From May 2017) Levy Contract":
                    case "16-18 Apprenticeship (Employer on App Service)":
                        if (employerType == ApprenticeshipEmployerType.Levy)
                            return "16-18 Apprenticeship (Employer on App Service) Levy funding";
                        else
                            return "16-18 Apprenticeship (Employer on App Service) Non-Levy funding";

                    case "19+ Apprenticeship (From May 2017) Levy Contract":
                    case "19+ Apprenticeship (Employer on App Service)":
                        if (employerType == ApprenticeshipEmployerType.Levy)
                            return "19+ Apprenticeship (Employer on App Service) Levy funding";
                        else
                            return "19+ Apprenticeship (Employer on App Service) Non-Levy funding";

                    default:
                        return $"None (unknown funding line type {fundingLineType})";
                }
            }

            if (contractType == ContractType.Act2)
            {
                switch (fundingLineType)
                {
                    case "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)":
                        return "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)";

                    case "16-18 Apprenticeship Non-Levy Contract (procured)":
                        return "16-18 Apprenticeship Non-Levy Contract (procured)";

                    case "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)":
                        return "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)";

                    case "19+ Apprenticeship Non-Levy Contract (procured)":
                        return "19+ Apprenticeship Non-Levy Contract (procured)";

                    default:
                        return $"None (unknown funding line type {fundingLineType})";
                }
            }

            return $"None (unknown contract type {contractType})";
        }
    }
}
