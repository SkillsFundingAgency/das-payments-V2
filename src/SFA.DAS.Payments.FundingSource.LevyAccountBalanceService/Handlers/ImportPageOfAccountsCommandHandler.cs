using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Messages.Commands;

namespace SFA.DAS.Payments.FundingSource.LevyAccountBalanceService.Handlers
{
    public class ImportPageOfAccountsCommandHandler : IHandleMessages<ImportPageOfAccounts>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IProcessLevyAccountBalanceService levyAccountBalanceService;
        
        public ImportPageOfAccountsCommandHandler(IPaymentLogger paymentLogger, IProcessLevyAccountBalanceService levyAccountBalanceService)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.levyAccountBalanceService = levyAccountBalanceService ?? throw new ArgumentNullException(nameof(levyAccountBalanceService));
        }

        public async Task Handle(ImportPageOfAccounts message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Import Page of Accounts Message Id : {context.MessageId}");

            await levyAccountBalanceService.RefreshLevyAccountDetails(message.PageNumber);

            paymentLogger.LogInfo($"Successfully processed Import Page of Accounts Message Id : {context.MessageId}");
        }
    }
}