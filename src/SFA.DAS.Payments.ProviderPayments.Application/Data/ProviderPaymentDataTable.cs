using System;
using System.Data;
using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Data
{
    public class ProviderPaymentDataTable : PeriodisedPaymentsEventModelDataTable<ProviderPaymentEventModel>
    {
        public ProviderPaymentDataTable()
        {
            DataTable.Columns.AddRange(new[]
            {
                new DataColumn("FundingSourceId", typeof(Guid)),
                new DataColumn("FundingSourceType"),
            });
        }

        protected override DataRow CreateDataRow(ProviderPaymentEventModel eventModel)
        {
            var dataRow = base.CreateDataRow(eventModel);
            dataRow["FundingSourceId"] = eventModel.FundingSourceId;
            dataRow["FundingSourceType"] = (byte)eventModel.FundingSource;
            return dataRow;
        }

        public override string TableName => "Payments2.Payment";
    }
}