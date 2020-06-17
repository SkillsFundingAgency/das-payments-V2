using System;
using System.Collections.Generic;
using FluentValidation;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.LevyAccountData
{
    public class CombinedLevyAccountValidator : AbstractValidator<CombinedLevyAccountsDto>
    {
        private readonly ITelemetry telemetry;

        public CombinedLevyAccountValidator(ITelemetry telemetry, IValidator<LevyAccountsDto> levyAccountValidator)
        {
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            if (levyAccountValidator == null) throw new ArgumentNullException(nameof(levyAccountValidator));

            RuleFor(act => act.IsNullOrEmpty)
                .Equal(false)
                .OnFailure(LogInvalidData);

            When(act => !act.IsNullOrEmpty,
                 () =>
                 {
                     RuleFor(act => act)
                         .Must(act => false) //fake rule to raise bellow events all the time
                         .OnFailure(RaiseCoreEmployerAccountReferenceDataEvents);

                     RuleForEach(act => act.LevyAccounts)
                         .SetValidator(levyAccountValidator);
                 });
        }

        private void LogInvalidData(CombinedLevyAccountsDto act)
        {
            telemetry.TrackEvent("EmployerAccountReferenceData.Comparison.InvalidData", new Dictionary<string, string>
             {
                 { "LevyAccount", "Either Das or Payments levy account Data is invalid, Please check the logs" }
             }, null);
        }
        
        private void RaiseCoreEmployerAccountReferenceDataEvents(CombinedLevyAccountsDto combinedDto)
        {
            telemetry.TrackEvent("EmployerAccountReferenceData.Comparison.LevyAccountCount", new Dictionary<string, double>
            {
                { "das-LevyAccountCount", Convert.ToDouble(combinedDto.DasLevyAccountCount) },
                { "payments-LevyAccountCount", Convert.ToDouble(combinedDto.PaymentsLevyAccountCount) },
            });
        
            telemetry.TrackEvent("EmployerAccountReferenceData.Comparison.IsLevyPayerCount", new Dictionary<string, double>
             {
                 { "das-IsLevyPayerCount", Convert.ToDouble(combinedDto.DasIsLevyPayerCount) },
                 { "payments-IsLevyPayerCount", Convert.ToDouble(combinedDto.PaymentsIsLevyPayerCount) },
             });
        
            telemetry.TrackEvent("EmployerAccountReferenceData.Comparison.TransferAllowanceTotal", new Dictionary<string, double>
            {
                { "das-TransferAllowanceTotal", Convert.ToDouble(combinedDto.DasTransferAllowanceTotal) },
                { "payments-TransferAllowanceTotal", Convert.ToDouble(combinedDto.PaymentsTransferAllowanceTotal) },
            });
        
            telemetry.TrackEvent("EmployerAccountReferenceData.Comparison.LevyAccountBalanceTotal", new Dictionary<string, double>
            {
                { "das-LevyAccountBalanceTotal", Convert.ToDouble(combinedDto.DasLevyAccountBalanceTotal) },
                { "payments-LevyAccountBalanceTotal", Convert.ToDouble(combinedDto.PaymentsLevyAccountBalanceTotal) },
            });
        }
    }
}
