﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.ProviderAdjustments.Application.Repositories;
using SFA.DAS.Payments.ProviderAdjustments.Domain;

namespace SFA.DAS.Payments.ProviderAdjustments.Application
{
    public interface IProviderAdjustmentsProcessor
    {
        Task ProcessEasAtMonthEnd(long jobId, int academicYear, int collectionPeriod);
    }

    public class ProviderAdjustmentsProcessor : IProviderAdjustmentsProcessor
    {
        private readonly IProviderAdjustmentRepository repository;
        private readonly IPaymentLogger logger;
        private readonly IProviderAdjustmentsCalculator calculator;
        private readonly ITelemetry telemetry;

        public ProviderAdjustmentsProcessor(
            IProviderAdjustmentRepository repository,
            IPaymentLogger logger,
            IProviderAdjustmentsCalculator calculator,
            ITelemetry telemetry)
        {
            this.repository = repository;
            this.logger = logger;
            this.calculator = calculator;
            this.telemetry = telemetry;
        }

        public async Task ProcessEasAtMonthEnd(long jobId, int academicYear, int collectionPeriod)
        {
            var properties = new Dictionary<string, string>
            {
                { TelemetryKeys.JobId, jobId.ToString()},
                { TelemetryKeys.CollectionPeriod, collectionPeriod.ToString()},
                { TelemetryKeys.AcademicYear, academicYear.ToString()},
            };

            try
            {
                logger.LogInfo("Started the Provider Adjustments Processor.");

                var historicPayments = await repository.GetPreviousProviderAdjustments(academicYear).ConfigureAwait(false);
                logger.LogInfo($"Found {historicPayments.Count} historic payments");

                var currentPayments = await repository.GetCurrentProviderAdjustments(academicYear).ConfigureAwait(false);
                logger.LogInfo($"Found {currentPayments.Count} payments from API");

                var payments = calculator.CalculateDelta(historicPayments, currentPayments, academicYear, collectionPeriod);
                logger.LogInfo($"Calculated {payments.Count} new payments");

                await repository.AddProviderAdjustments(payments).ConfigureAwait(false);

                logger.LogInfo("Finished the Provider Adjustments Processor.");

                SendEASTelemetry(true, properties, historicPayments, currentPayments);
            }
            catch (Exception)
            {
                SendEASTelemetry(false, properties);
                throw;
            }
        }

        private void SendEASTelemetry(bool isSuccessful, Dictionary<string, string> properties, List<ProviderAdjustment> historicPayments = null, List<ProviderAdjustment> currentPayments = null)
        {
            var stats = new Dictionary<string, double>
            {
                {"HistoricPayments", historicPayments?.Count ?? 0 },
                {"CurrentPayments", currentPayments?.Count ?? 0 }
            };

            properties.Add("isSuccessful", isSuccessful.ToString().ToLower());

            telemetry.TrackEvent("Finished processing EAS", properties, stats);
        }
    }
}
