using System.Collections.Generic;
using System.Globalization;
using FluentValidation;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring
{
    public class LevyAccountValidator : AbstractValidator<LevyAccountsDto>
    {
        private readonly ITelemetry telemetry;

        public LevyAccountValidator(ITelemetry telemetry)
        {
            this.telemetry = telemetry;
            RuleFor(la => la.DasLevyAccount.AccountId)
                .NotEmpty()
                .OnFailure(LogMissingDasAccount);

            RuleFor(la => la.PaymentsLevyAccount.AccountId)
                .NotEmpty()
                .OnFailure(LogMissingPaymentsAccount);

            RuleFor(la => la.DasLevyAccount.Balance)
                .Equal(la => la.PaymentsLevyAccount.Balance)
                .OnFailure(LogBalanceMismatch);

            RuleFor(la => la.DasLevyAccount.TransferAllowance)
                .Equal(la => la.PaymentsLevyAccount.TransferAllowance)
                .OnFailure(LogTransferAllowanceMismatch);
            
            RuleFor(la => la.DasLevyAccount.IsLevyPayer)
                .Equal(la => la.PaymentsLevyAccount.IsLevyPayer)
                .OnFailure(LogIsLevyPayerMismatch);
        }

        private long GetAccountId(LevyAccountsDto levyAccountsDto)
        {
            return levyAccountsDto.DasLevyAccount.AccountId != 0 ? levyAccountsDto.DasLevyAccount.AccountId : levyAccountsDto.PaymentsLevyAccount.AccountId;
        }
        
        private void LogMissingPaymentsAccount(LevyAccountsDto levyAccountsDto)
        {
            telemetry.TrackEvent("EmployerAccountReferenceData.Comparison.MissingLevyAccount", new Dictionary<string, string>
            {
                { "Payments-MissingLevyAccount", $"LevyAccount with Id { GetAccountId(levyAccountsDto) } missing in Payments System" }
            }, null);
        }

        private void LogMissingDasAccount(LevyAccountsDto levyAccountsDto)
        {
            telemetry.TrackEvent("EmployerAccountReferenceData.Comparison.MissingLevyAccount", new Dictionary<string, string>
            {
                { "Das-MissingLevyAccount", $"LevyAccount with Id { GetAccountId(levyAccountsDto) } missing in EAS System" }
            }, null);
        }
        
        private void LogIsLevyPayerMismatch(LevyAccountsDto levyAccountsDto)
        {
            telemetry.TrackEvent("EmployerAccountReferenceData.Comparison.IsLevyPayerMismatch", new Dictionary<string, string>
            {
                { "IsLevyPayerMismatch", $"IsLevyPayer Does not match for LevyAccountId { GetAccountId(levyAccountsDto) }" },
                { "Das-IsLevyPayer", levyAccountsDto.DasLevyAccount.IsLevyPayer.ToString() },
                { "Payments-IsLevyPayer", levyAccountsDto.PaymentsLevyAccount.IsLevyPayer.ToString() }
            }, null);
        }

        private void LogTransferAllowanceMismatch(LevyAccountsDto levyAccountsDto)
        {
            telemetry.TrackEvent("EmployerAccountReferenceData.Comparison.TransferAllowanceMismatch", new Dictionary<string, string>
            {
                { "TransferAllowanceMismatch", $"TransferAllowance Does not match for LevyAccountId { GetAccountId(levyAccountsDto) }" },
                { "Das-TransferAllowance", levyAccountsDto.DasLevyAccount.TransferAllowance.ToString(CultureInfo.InvariantCulture) },
                { "Payments-TransferAllowance", levyAccountsDto.PaymentsLevyAccount.TransferAllowance.ToString(CultureInfo.InvariantCulture) }
            }, null);
        }

        private void LogBalanceMismatch(LevyAccountsDto levyAccountsDto)
        {
            telemetry.TrackEvent("EmployerAccountReferenceData.Comparison.BalanceMismatch", new Dictionary<string, string>
            {
                { "BalanceMismatch", $"Balance Does not match for LevyAccountId { GetAccountId(levyAccountsDto) }" },
                { "Das-Balance", levyAccountsDto.DasLevyAccount.Balance.ToString(CultureInfo.InvariantCulture) },
                { "Payments-Balance", levyAccountsDto.PaymentsLevyAccount.Balance.ToString(CultureInfo.InvariantCulture) }
            }, null);
        }
    }
}
