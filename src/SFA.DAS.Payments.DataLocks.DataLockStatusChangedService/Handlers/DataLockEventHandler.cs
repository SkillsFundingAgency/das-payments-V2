using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.DataLocks.Application.Services;

namespace SFA.DAS.Payments.DataLocks.DataLockStatusChangedService.Handlers
{
    public class DataLockEventHandler : IHandleMessages<DataLockEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IManageReceivedDataLockEvent manageReceivedDataLockEvent;
        private readonly List<short> academicYearsToIgnore = new List<short>();
        
        public DataLockEventHandler(IPaymentLogger paymentLogger, IManageReceivedDataLockEvent manageReceivedDataLockEvent,
            IConfigurationHelper configuration)
        {
            this.paymentLogger = paymentLogger;
            this.manageReceivedDataLockEvent = manageReceivedDataLockEvent;
            var academicYearsToIgnoreConfiguration = configuration.GetSetting("IgnoreApprovalsPriceEpisodeEventsForAcademicYears");
            try
            {
                var ignoredYearsAsAList = academicYearsToIgnoreConfiguration.Split(',')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => x.Trim())
                    .Select(x => Convert.ToInt16(x));

                academicYearsToIgnore.AddRange(ignoredYearsAsAList);
            }
            catch (FormatException)
            {
                paymentLogger.LogWarning("Issue with the 'IgnoreApprovalsPriceEpisodeEventsForAcademicYears' configuration setting");
            }
        }

        public async Task Handle(DataLockEvent message, IMessageHandlerContext context)
        {
            if (academicYearsToIgnore.Contains(message.CollectionYear))
            {
                paymentLogger.LogInfo($"Ignoring DataLockEvent for year: {message.CollectionYear}");
                return;
            }

            paymentLogger.LogVerbose($"Processing {message.GetType().Name} event for UKPRN {message.Ukprn}");
            await manageReceivedDataLockEvent.ProcessDataLockEvent(message);
            paymentLogger.LogVerbose($"Successfully Processed {message.GetType().Name} event for UKPRN {message.Ukprn}");
        }
    }
}
