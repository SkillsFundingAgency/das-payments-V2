using System.Threading.Tasks;
using System.Web;
using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Submissions.Functions.Infrastructure.IoC;

namespace SFA.DAS.Payments.Monitoring.Submissions.Functions
{
    [DependencyInjectionConfig(typeof(DependencyInjectionConfig))]
    public static class SuccessfulSubmissions
    {
        [FunctionName("SuccessfulSubmission")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/Submission/Successful")] HttpRequest req,
            [Inject]IPaymentLogger log)
        {
            var queryString = HttpUtility.ParseQueryString(req.QueryString.Value);
            var academicYear = queryString.Get("academicYear");
            var collectionPeriod = queryString.Get("collectionPeriod");
        }
    }
}
