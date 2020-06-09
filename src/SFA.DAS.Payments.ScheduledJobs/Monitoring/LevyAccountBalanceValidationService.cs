using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring
{
    public interface ILevyAccountBalanceValidationService
    {
        Task Validate();
    }
    
    public class LevyAccountBalanceValidationService : ILevyAccountBalanceValidationService
    {
        private readonly IDasLevyAccountBalanceApiWrapper dasLevyAccountBalanceApiWrapper;
        private readonly IPaymentsDataContext paymentsDataContext;
        private readonly IValidator<CombinedLevyAccountsDto> validator;
        private readonly IPaymentLogger paymentLogger;

        public LevyAccountBalanceValidationService(
            IDasLevyAccountBalanceApiWrapper dasLevyAccountBalanceApiWrapper,
            IPaymentsDataContext paymentsDataContext,
            IValidator<CombinedLevyAccountsDto> validator,
            IPaymentLogger paymentLogger)
        {
            this.dasLevyAccountBalanceApiWrapper = dasLevyAccountBalanceApiWrapper;
            this.paymentsDataContext = paymentsDataContext;
            this.validator = validator;
            this.paymentLogger = paymentLogger;
        }

        public async Task Validate()
        {
            var combinedLevyAccountBalance = await GetLevyAccountDetails();
            
            paymentLogger.LogInfo("Started Validating Employer Accounts");

            var res = await validator.ValidateAsync(combinedLevyAccountBalance);
        
            paymentLogger.LogInfo("Finished Validating Employer Accounts");
        }

        private async Task<CombinedLevyAccountsDto> GetLevyAccountDetails()
        {
            var dasLevyAccountDetails = GetDasLevyAccountDetails();
            var paymentsLevyAccountDetails = GetPaymentsLevyAccountDetails();

            await Task.WhenAll(dasLevyAccountDetails, paymentsLevyAccountDetails).ConfigureAwait(false);

            return new CombinedLevyAccountsDto(dasLevyAccountDetails.Result, paymentsLevyAccountDetails.Result);
        }

        private async Task<List<LevyAccountModel>> GetDasLevyAccountDetails()
        {
            return await dasLevyAccountBalanceApiWrapper.GetDasLevyAccountDetails();
        }

        private async Task<List<LevyAccountModel>> GetPaymentsLevyAccountDetails()
        {
            paymentLogger.LogInfo("Started Importing Payments Employer Accounts");

            var levyAccountModels = await paymentsDataContext.LevyAccount.ToListAsync();

            paymentLogger.LogInfo("Finished Importing Payments Employer Accounts");

            return levyAccountModels;
        }
    }
}
