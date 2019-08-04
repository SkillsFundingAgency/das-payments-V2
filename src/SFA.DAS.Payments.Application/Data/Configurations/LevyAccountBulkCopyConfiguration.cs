using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class LevyAccountBulkCopyConfiguration : IBulkCopyConfiguration<LevyAccountModel>
    {

        private readonly IConfigurationHelper configurationHelper;
        private static readonly IDictionary<string, string> ColumnList = typeof(LevyAccountModel).GetProperties().ToDictionary(p => p.Name, p => p.Name);

        public LevyAccountBulkCopyConfiguration(IConfigurationHelper configurationHelper)
        {
            this.configurationHelper = configurationHelper;
        }
        
        public string TableName => "[Payments2].[LevyAccount]";

        public IDictionary<string, string> Columns => ColumnList;
        public string ConnectionString => configurationHelper.GetConnectionString("PaymentsConnectionString");
    }
}