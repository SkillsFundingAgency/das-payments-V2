using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class LevyAccountBulkCopyConfiguration : IBulkDeleteAndCopyConfiguration<LevyAccountModel>, IBulkCopyConfiguration<LevyAccountModel>
    {
        private static readonly IDictionary<string, string> ColumnList = typeof(LevyAccountModel).GetProperties().ToDictionary(p => p.Name, p => p.Name);

        public string TableName => "[Payments2].[LevyAccount]";

        public IDictionary<string, string> Columns => ColumnList;
        public string BulkDeleteFilterColumnName => nameof(LevyAccountModel.AccountId);
    }
}