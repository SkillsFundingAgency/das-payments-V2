using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using NServiceBus;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;

// ReSharper disable UnusedMember.Global

namespace SFA.DAS.Payments.ScheduledJobs.AuditDataCleanUp
{
    [DependencyInjectionConfig(typeof(DependencyInjectionConfig))]
    public static class AuditDataCleanUpTrigger
    {
        [FunctionName("TimerTriggerAuditDataCleanup")]
        public static async Task TimerTriggerAuditDataCleanup(
            [TimerTrigger("%AuditDataCleanUpSchedule%", RunOnStartup = false)] TimerInfo timerInfo,
            [Inject]IAuditDataCleanUpService auditDataCleanUpService,
            [Inject]IEndpointInstanceFactory endpointInstanceFactory,
            [Inject]IScheduledJobsConfiguration config)
        {
            await TriggerAuditDataCleanup(auditDataCleanUpService, endpointInstanceFactory, config);
        }

        [FunctionName("HttpTriggerAuditDataCleanup")]
        public static async Task HttpTriggerAuditDataCleanup(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest httpRequest,
            [Inject]IAuditDataCleanUpService auditDataCleanUpService,
            [Inject]IEndpointInstanceFactory endpointInstanceFactory,
            [Inject]IScheduledJobsConfiguration config)
        {
            await TriggerAuditDataCleanup(auditDataCleanUpService, endpointInstanceFactory, config);
        }

        private static async Task TriggerAuditDataCleanup(
            IAuditDataCleanUpService auditDataCleanUpService,
            IEndpointInstanceFactory endpointInstanceFactory,
            IScheduledJobsConfiguration config)
        {
            var submissionJobsToBeDeletedBatches = await auditDataCleanUpService.GetSubmissionJobsToBeDeletedBatches(config.CollectionPeriod, config.AcademicYear);
            var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);

            foreach (var batch in submissionJobsToBeDeletedBatches)
            {
                await endpointInstance.Send(config.EarningAuditDataCleanUpQueue, batch).ConfigureAwait(false);
                await endpointInstance.Send(config.DataLockAuditDataCleanUpQueue, batch).ConfigureAwait(false);
                await endpointInstance.Send(config.FundingSourceAuditDataCleanUpQueue, batch).ConfigureAwait(false);
                await endpointInstance.Send(config.RequiredPaymentAuditDataCleanUpQueue, batch).ConfigureAwait(false);
            }
        }
    }
}
