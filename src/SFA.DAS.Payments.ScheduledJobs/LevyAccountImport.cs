using System;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;
// ReSharper disable UnusedMember.Global

namespace SFA.DAS.Payments.ScheduledJobs
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class LevyAccountImport
    {
        [FunctionName("LevyAccountImport")]
        public static async Task Run([TimerTrigger("%LevyAccountSchedule%", RunOnStartup=true)]TimerInfo myTimer, [Inject]IEndpointInstanceFactory endpointInstanceFactory, [Inject]IScheduledJobsConfiguration config, ILogger log)
        {
            try
            {
                var command = new ImportEmployerAccounts();
                var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);
                await endpointInstance.Send(config.LevyAccountBalanceEndpoint, command).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                log.Log(LogLevel.Error, e, "Error in LevyAccountImport");
                throw;
            }
        }
    }
}
