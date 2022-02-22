using System;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;
// ReSharper disable UnusedMember.Global

namespace SFA.DAS.Payments.ScheduledJobs
{
    [DependencyInjectionConfig(typeof(DependencyInjectionConfig))]
    public static class LevyAccountImport
    {
        [FunctionName("LevyAccountImport")]
        public static async Task Run([TimerTrigger("%LevyAccountSchedule%", RunOnStartup=false)]TimerInfo myTimer, [Inject]IEndpointInstanceFactory endpointInstanceFactory, [Inject]IScheduledJobsConfiguration config, [Inject]IPaymentLogger log)
        {
            await RunLevyAccountImport(endpointInstanceFactory, config, log);
        }

        [FunctionName("HttpLevyAccountImport")]
        public static async Task HttpLevyAccountImport([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, [Inject]IEndpointInstanceFactory endpointInstanceFactory, [Inject]IScheduledJobsConfiguration config, [Inject]IPaymentLogger log)
        {
            await RunLevyAccountImport(endpointInstanceFactory, config, log);
        }
        
        private static async Task RunLevyAccountImport(IEndpointInstanceFactory endpointInstanceFactory, IScheduledJobsConfiguration config, IPaymentLogger log)
        {
            try
            {
                var command = new ImportEmployerAccounts();
                var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);
                await endpointInstance.Send(config.LevyAccountBalanceEndpoint, command).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                log.LogError("Error in LevyAccountImport", e);
                throw;
            }
        }
    }
}
