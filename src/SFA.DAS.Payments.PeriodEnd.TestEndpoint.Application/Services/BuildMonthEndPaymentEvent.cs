using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.PeriodEnd.TestEndpoint.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.PeriodEnd.TestEndpoint.Application.Services
{
    public class BuildMonthEndPaymentEvent :IBuildMonthEndPaymentEvent
    {
        private readonly ITestEndPointRepository testEndPointRepository;
        private readonly IApprenticeshipKeyService apprenticeshipKeyService;
        private readonly Random random;

        public BuildMonthEndPaymentEvent(ITestEndPointRepository testEndPointRepository, IApprenticeshipKeyService apprenticeshipKeyService)
        {
            this.testEndPointRepository = testEndPointRepository;
            this.apprenticeshipKeyService = apprenticeshipKeyService;
            random = new Random();
        }

        public async Task<CollectionStartedEvent> CreateCollectionStartedEvent(long ukprn, short academicYear)
        {
            var providerAims = await testEndPointRepository
                .GetProviderLearnerAims(ukprn)
                .ConfigureAwait(false);

            var apprenticeshipKeys = providerAims.Select(aim =>
                    apprenticeshipKeyService.GenerateApprenticeshipKey(
                        ukprn,
                        aim.LearnerReferenceNumber,
                        aim.LearningAimFrameworkCode,
                        aim.LearningAimPathwayCode,
                        aim.LearningAimProgrammeType,
                        aim.LearningAimStandardCode,
                        aim.LearningAimReference,
                        academicYear))
                .ToList();

            return new CollectionStartedEvent
            {
                ApprenticeshipKeys = apprenticeshipKeys,
                JobId = GenerateId()
            };
        }

        public ProcessProviderMonthEndCommand CreateProcessProviderMonthEndCommand(long ukprn, short academicYear, byte period)
        {
            return  new ProcessProviderMonthEndCommand
            {
                Ukprn = ukprn,
                JobId = GenerateId(),
                CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = academicYear,
                    Period = period
                },
            };
        }
        private long GenerateId(int maxValue = 1000000000)
        {
            var id = random.Next(maxValue);
            return id;
        }
    }
}