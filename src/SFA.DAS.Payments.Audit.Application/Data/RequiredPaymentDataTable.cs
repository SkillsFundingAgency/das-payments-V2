using System;
using System.Data;
using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.Audit.Application.Data
{
    public class RequiredPaymentDataTable : PeriodisedPaymentsEventModelDataTable<RequiredPaymentEventModel>
    {
        public RequiredPaymentDataTable()
        {
        }

        protected override DataRow CreateDataRow(RequiredPaymentEventModel eventModel)
        {
            var dataRow = base.CreateDataRow(eventModel);
            return dataRow;
        }

        public override string TableName => "Payments2.RequiredPaymentEvent";
    }
}