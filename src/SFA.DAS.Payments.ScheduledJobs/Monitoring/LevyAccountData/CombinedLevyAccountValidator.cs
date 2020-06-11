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
            if(levyAccountValidator == null) throw new ArgumentNullException(nameof(levyAccountValidator));
            
            RuleFor(act => act.DasLevyAccountCount)
                .Equal(act => act.PaymentsLevyAccountCount)
                .OnFailure(act => LogCountMismatch(act.DasLevyAccountCount, act.PaymentsLevyAccountCount));

            RuleFor(act => act.DasLevyAccountBalanceTotal)
                .Equal(act => act.PaymentsLevyAccountBalanceTotal)
                .OnFailure(act => LogBalanceMismatch(act.DasLevyAccountBalanceTotal, act.PaymentsLevyAccountBalanceTotal));

            RuleFor(act => act.DasTransferAllowanceTotal)
                .Equal(act => act.PaymentsTransferAllowanceTotal)
                .OnFailure(act => LogTransferAllowanceMismatch(act.DasTransferAllowanceTotal, act.PaymentsTransferAllowanceTotal));

            RuleFor(act => act.DasIsLevyPayerCount)
                .Equal(act => act.PaymentsIsLevyPayerCount)
                .OnFailure(act => LogIsLevyPayerMismatch(act.DasIsLevyPayerCount, act.PaymentsIsLevyPayerCount));

            RuleForEach(act => act.LevyAccounts)
                .SetValidator(levyAccountValidator);
        }

        
        private void LogCountMismatch(int dasLevyAccountCount, int paymentsLevyAccountCount)
        {
            telemetry.TrackEvent("EmployerAccountReferenceData.Comparison.LevyAccountCount", new Dictionary<string, double>
            {
                { "das-LevyAccountCount", Convert.ToDouble(dasLevyAccountCount) },
                { "payments-LevyAccountCount", Convert.ToDouble(paymentsLevyAccountCount) },
            });
        }
        
        private void LogIsLevyPayerMismatch(decimal dasIsLevyPayerCount, decimal paymentsIsLevyPayerCount)
        {
            telemetry.TrackEvent("EmployerAccountReferenceData.Comparison.IsLevyPayerCount", new Dictionary<string, double>
             {
                 { "das-IsLevyPayerCount", Convert.ToDouble(dasIsLevyPayerCount) },
                 { "payments-IsLevyPayerCount", Convert.ToDouble(paymentsIsLevyPayerCount) },
             });
        }

        private void LogTransferAllowanceMismatch(decimal dasTransferAllowanceTotal, decimal paymentsTransferAllowanceTotal)
        {
            telemetry.TrackEvent("EmployerAccountReferenceData.Comparison.TransferAllowanceTotal", new Dictionary<string, double>
            {
                { "das-TransferAllowanceTotal", Convert.ToDouble(dasTransferAllowanceTotal) },
                { "payments-TransferAllowanceTotal", Convert.ToDouble(paymentsTransferAllowanceTotal) },
            });
        }

        private void LogBalanceMismatch(decimal dasLevyAccountBalanceTotal, decimal paymentsLevyAccountBalanceTotal)
        {
            telemetry.TrackEvent("EmployerAccountReferenceData.Comparison.LevyAccountBalanceTotal", new Dictionary<string, double>
            {
                { "das-LevyAccountBalanceTotal", Convert.ToDouble(dasLevyAccountBalanceTotal) },
                { "payments-LevyAccountBalanceTotal", Convert.ToDouble(paymentsLevyAccountBalanceTotal) },
            });
        }
    }
}
