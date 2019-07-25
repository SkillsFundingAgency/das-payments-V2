using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.EarningEvents.Model.Entities;

namespace SFA.DAS.Payments.EarningEvents.Application.Repositories
{
    public class SubmissionEventBulkCopyConfiguration : IBulkCopyConfiguration<LegacySubmissionEvent>
    {
        private readonly IConfigurationHelper configurationHelper;

        public SubmissionEventBulkCopyConfiguration(IConfigurationHelper configurationHelper)
        {
            this.configurationHelper = configurationHelper;
            TableName = "[Submissions].[SubmissionEvents]";
            Columns = typeof(LegacySubmissionEvent).GetProperties().ToDictionary(p => p.Name, p => p.Name);
        }

        public IDictionary<string, string> Columns { get; }

        public string TableName { get; }

        public string ConnectionString => configurationHelper.GetConnectionString("ProviderEventsConnectionString");
    }
}