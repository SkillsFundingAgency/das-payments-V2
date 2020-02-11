using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ProviderAdjustments.Application.Repositories;
using SFA.DAS.Payments.ProviderAdjustments.Domain;

namespace SFA.DAS.Payments.ProviderAdjustments.Application
{
    public interface IProviderAdjustmentsProcessor
    {
        Task ProcessEasAtMonthEnd(int academicYear, int collectionPeriod);
    }

    public class ProviderAdjustmentsProcessor : IProviderAdjustmentsProcessor
    {
        private readonly IProviderAdjustmentRepository repository;
        private readonly IPaymentLogger logger;
        private readonly IProviderAdjustmentsCalculator calculator;
        
        public ProviderAdjustmentsProcessor(
            IProviderAdjustmentRepository repository, 
            IPaymentLogger logger,
            IProviderAdjustmentsCalculator calculator)
        {
            this.repository = repository;
            this.logger = logger;
            this.calculator = calculator;
        }

        public async Task ProcessEasAtMonthEnd(int academicYear, int collectionPeriod)
        {
            logger.LogInfo("Started the Provider Adjustments Processor.");

            var historicPayments = await repository.GetPreviousProviderAdjustments(academicYear).ConfigureAwait(false);
            logger.LogInfo($"Found {historicPayments.Count} historic payments");

            var currentPayments = await repository.GetCurrentProviderAdjustments(academicYear).ConfigureAwait(false);
            logger.LogInfo($"Found {currentPayments.Count} payments from API");

            var payments = calculator.CalculateDelta(historicPayments, currentPayments);
            logger.LogInfo($"Calculated {payments.Count} new payments");

            PopulateCollectionPeriod(payments, academicYear, collectionPeriod);
            await repository.AddProviderAdjustments(payments).ConfigureAwait(false);

            logger.LogInfo("Finished the Provider Adjustments Processor.");
        }

        private void PopulateCollectionPeriod(List<ProviderAdjustment> payments, int academicYear, int collectionPeriod)
        {
            var collectionPeriodMonth = collectionPeriod < 6 ? collectionPeriod + 7 : collectionPeriod - 5;
            var collectionPeriodYear = collectionPeriod < 6 ? 2000 + academicYear / 100 : 2000 + academicYear / 100 + 1;
            var collectionPeriodName = $"{academicYear}-R{collectionPeriod:D2}";

            foreach (var payment in payments)
            {
                payment.CollectionPeriodMonth = collectionPeriodMonth;
                payment.CollectionPeriodYear = collectionPeriodYear;
                payment.CollectionPeriodName = collectionPeriodName;
                payment.SubmissionAcademicYear = academicYear;
            }
        }
    }
}
