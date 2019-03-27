using System;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Application.Services
{

    public class ApprenticeshipKeyFactory : IApprenticeshipKeyFactory
    {
        private ApprenticeshipKey apprenticeshipKey;
        private readonly IApprenticeshipKeyService apprenticeshipKeyService;

        public ApprenticeshipKeyFactory(IApprenticeshipKeyService apprenticeshipKeyService)
        {
            this.apprenticeshipKeyService = apprenticeshipKeyService;
        }

        public void SetCurrentKey(string key)
        {
            apprenticeshipKey = apprenticeshipKeyService.ParseApprenticeshipKey(key);
        }

        public ApprenticeshipKey GetCurrentKey()
        {
            if (apprenticeshipKey == null)
                throw new ApplicationException("Apprenticeship Key is not set");

            return apprenticeshipKey;
        }
    }
}
