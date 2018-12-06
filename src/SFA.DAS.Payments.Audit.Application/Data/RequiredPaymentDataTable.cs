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
                new DataColumn("PaymentsDueId", typeof(Guid)),
            });
        }

        protected override DataRow CreateDataRow(RequiredPaymentEventModel eventModel)
        {
            var dataRow = base.CreateDataRow(eventModel);
            dataRow["PaymentsDueId"] = eventModel.PaymentsDueEventId;
            return dataRow;
        }

        public override string TableName => "Payments2.RequiredPaymentEvent";
    }
}