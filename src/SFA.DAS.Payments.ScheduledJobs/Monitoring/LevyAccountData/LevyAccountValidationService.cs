using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.LevyAccountData
{
    public interface ILevyAccountValidationService
    {
        Task Validate();
    }
    
    public class LevyAccountValidationService : ILevyAccountValidationService
    {
        private readonly IDasLevyAccountApiWrapper dasLevyAccountApiWrapper;
        private readonly IPaymentsDataContext paymentsDataContext;
        private readonly IValidator<CombinedLevyAccountsDto> validator;
        private readonly IPaymentLogger paymentLogger;

        public LevyAccountValidationService(
            IDasLevyAccountApiWrapper dasLevyAccountApiWrapper,
            IPaymentsDataContext paymentsDataContext,
            IValidator<CombinedLevyAccountsDto> validator,
            IPaymentLogger paymentLogger)
        {
            this.dasLevyAccountApiWrapper = dasLevyAccountApiWrapper ?? throw new ArgumentNullException(nameof(dasLevyAccountApiWrapper));
            this.paymentsDataContext = paymentsDataContext ?? throw new ArgumentNullException(nameof(paymentsDataContext));
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
        }

        public async Task Validate()
        {
            var combinedLevyAccountBalance = await GetLevyAccountDetails();
            
            paymentLogger.LogDebug("Started Validating Employer Accounts");

            await validator.ValidateAsync(combinedLevyAccountBalance);
        
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
            return await dasLevyAccountApiWrapper.GetDasLevyAccountDetails();
        }

        private async Task<List<LevyAccountModel>> GetPaymentsLevyAccountDetails()
        {
            paymentLogger.LogDebug("Started Importing Payments Employer Accounts");

            List<LevyAccountModel> levyAccountModels;

            try
            {
                levyAccountModels = await paymentsDataContext.LevyAccount.ToListAsync();
                if (levyAccountModels.IsNullOrEmpty())
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                paymentLogger.LogError("Error while retrieving Account Balance Details from PaymentsV2", e);
                return null;
            }

            paymentLogger.LogInfo("Finished Importing Payments Employer Accounts");

            return levyAccountModels;
        }
    }
}
