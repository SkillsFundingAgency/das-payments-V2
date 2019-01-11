using System;
using System.Data;
using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.Audit.Application.Data
{
    public class RequiredPaymentDataTable : PeriodisedPaymentsEventModelDataTable<RequiredPaymentEventModel>
    {
        public RequiredPaymentDataTable()
        {
            DataTable.Columns.AddRange(new[]
            {
                new DataColumn("EarningEventId", typeof(Guid)),
            });
        }

        protected override DataRow CreateDataRow(RequiredPaymentEventModel eventModel)
        {
            var dataRow = base.CreateDataRow(eventModel);
            dataRow["EarningEventId"] = eventModel.EarningEventId;
            return dataRow;
        }

        public override string TableName => "Payments2.RequiredPaymentEvent";
    }
}