using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;

namespace SFA.DAS.Payments.ScheduledJobs
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class LevyAccountImport
    {
        [FunctionName("LevyAccountImport")]
        public static async Task Run([TimerTrigger("%LevyAccountSchedule%", RunOnStartup=true)]TimerInfo myTimer, [Inject]IEndpointInstanceFactory endpointInstanceFactory, ILogger log)
        {
            var command = new ImportEmployerAccounts();
            var sendOptions = new SendOptions();
            sendOptions.SetDestination("sfa-das-payments-levyaccountbalance");
            var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);
            await endpointInstance.Send(command, sendOptions).ConfigureAwait(false);
        }
    }
}
