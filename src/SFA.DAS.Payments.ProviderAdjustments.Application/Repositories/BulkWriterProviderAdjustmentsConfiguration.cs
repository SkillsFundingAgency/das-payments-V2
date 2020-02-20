using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ProviderAdjustments.Domain;

namespace SFA.DAS.Payments.ProviderAdjustments.Application.Repositories
{
    class BulkWriterProviderAdjustmentsConfiguration : IBulkCopyConfiguration<ProviderAdjustment>
    {
        public BulkWriterProviderAdjustmentsConfiguration(IConfigurationHelper configurationHelper)
        {
            ConnectionString = configurationHelper.GetConnectionString("PaymentsConnectionString");
            Columns = typeof(ProviderAdjustment).GetProperties().ToDictionary(p => p.Name, p => p.Name);
        }

        public string TableName { get; } = "[Payments2].[ProviderAdjustmentPayments]";
        public IDictionary<string, string> Columns { get; }
        public string ConnectionString { get; }
    }
}
