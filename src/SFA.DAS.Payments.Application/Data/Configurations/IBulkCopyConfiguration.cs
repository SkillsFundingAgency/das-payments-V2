using System.Collections.Generic;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public interface IBulkCopyConfiguration<T> where T : class
    {
        string TableName { get; }
        IDictionary<string, string> Columns { get; }
    }
    
    public interface IBulkDeleteAndCopyConfiguration<T>: IBulkCopyConfiguration<T> where T : class
    {
        string BulkDeleteFilterColumnName { get; }
    }
}