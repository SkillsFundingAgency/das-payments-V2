﻿using System.Collections.Generic;
using System.Data;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Data
{
    public interface IPaymentsEventModelDataTable<T> where T : IPaymentsEventModel
    {
        string TableName { get; }
        List<DataTable> GetDataTable(List<T> events);
    }
}