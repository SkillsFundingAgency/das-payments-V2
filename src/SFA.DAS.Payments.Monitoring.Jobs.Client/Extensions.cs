using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public static class Extensions
    {
        public static string GetMonitoringEndpointName(this IConfigurationHelper config, long jobId)
        {
            var jobsEndpointName = config.GetSettingOrDefault("Monitoring_JobsService_EndpointName", "sfa-das-payments-monitoring-jobs");
//            return $"{jobsEndpointName}{jobId % 20}";
            return $"{jobsEndpointName}0";
        }
    }
}