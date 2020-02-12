using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Messages.Commands;

namespace SFA.DAS.Payments.FundingSource.LevyAccountBalanceService.Handlers
{
    public class ImportEmployerAccountsCommandHandler : IHandleMessages<ImportEmployerAccounts>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IProcessLevyAccountBalanceService levyAccountBalanceService;
        
        public ImportEmployerAccountsCommandHandler(IPaymentLogger paymentLogger,
                                                                      IProcessLevyAccountBalanceService levyAccountBalanceService)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.levyAccountBalanceService = levyAccountBalanceService ?? throw new ArgumentNullException(nameof(levyAccountBalanceService));
        }

        public async Task Handle(ImportEmployerAccounts message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Import Employer Accounts Message Id : {context.MessageId}");

            await levyAccountBalanceService.RefreshLevyAccountDetails();

            paymentLogger.LogInfo($"Successfully processed Import Employer Accounts Message Id : {context.MessageId}");

        }
    }
}