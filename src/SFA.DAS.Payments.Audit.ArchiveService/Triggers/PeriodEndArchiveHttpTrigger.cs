﻿using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Extensions;
using SFA.DAS.Payments.Audit.ArchiveService.Helpers;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC;

namespace SFA.DAS.Payments.Audit.ArchiveService.Triggers
{
    [DependencyInjectionConfig(typeof(DependencyRegister))]
    public static class PeriodEndArchiveHttpTrigger
    {
        [FunctionName(nameof(PeriodEndArchiveHttpTrigger))]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post",
                Route = "orchestrators/PeriodEndArchiveOrchestrator")]
            HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            [DurableClient] IDurableEntityClient client,
            [Inject] IPaymentLogger log
        )
        {
            try
            {
                if (req.Method == HttpMethod.Post)
                {
                    if (req.Content != null)
                    {
                        var messageJson = await req.Content.ReadAsStringAsync();

                        ITriggerHelper triggerHelper = new TriggerHelper();
                        return await triggerHelper.StartOrchestrator(req, starter, log, client);
                    }

                    throw new Exception(
                        $"Error in PeriodEndArchiveHttpTrigger. Request content is null. Request: {req}");
                }

                var entityId = new EntityId(nameof(HandleCurrentJobId.Handle),
                    HandleCurrentJobId.PeriodEndArchiveEntityName);
                var stateResponse = await client.ReadEntityStateAsync<JObject>(entityId);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(stateResponse), Encoding.UTF8,
                        "application/json")
                };
            }

            catch (Exception ex)
            {
                log.LogError("Error in PeriodEndArchiveHttpTrigger", ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }
    }
}