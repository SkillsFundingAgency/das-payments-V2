using System;
using System.Collections.Generic;
using System.Data;
using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.Audit.Application.Data
{
    public class EarningEventDataTable : PaymentsEventModelDataTable<EarningEventModel>
    {
        private readonly DataTable periods;
        private readonly DataTable priceEpisodes;
        public override string TableName => "Payments2.EarningEvent";

        public EarningEventDataTable()
        {
            DataTable.TableName = "Payments2.EarningEvent";
            DataTable.Columns.AddRange(new[]
            {
                new DataColumn("ContractType"),
                new DataColumn("AgreementId"),
            });
            periods = new DataTable("Payments2.EarningEventPeriod");
            periods.Columns.AddRange(new[]
            {
                new DataColumn("EarningEventId", typeof(Guid)),
                new DataColumn("PriceEpisodeIdentifier"),
                new DataColumn("TransactionType"),
                new DataColumn("DeliveryPeriod"),
                new DataColumn("Amount"),
            });
            priceEpisodes = new DataTable("Payments2.EarningEventPriceEpisode");
            priceEpisodes.Columns.AddRange(new[]
            {
                new DataColumn("EarningEventId", typeof(Guid)),
                new DataColumn("PriceEpisodeIdentifier"),
                new DataColumn("SfaContributionPercentage"),
                new DataColumn("StartDate"),
                new DataColumn("PlannedEndDate"),
                new DataColumn("ActualEndDate"),
                new DataColumn("TotalNegotiatedPrice1"),
                new DataColumn("TotalNegotiatedPrice2"),
                new DataColumn("TotalNegotiatedPrice3"),
                new DataColumn("TotalNegotiatedPrice4"),
                new DataColumn("Completed"),
                new DataColumn("CompletionAmount"),
                new DataColumn("InstalmentAmount"),
                new DataColumn("NumberOfInstalments"),
            });
        }

        protected override DataRow CreateDataRow(EarningEventModel eventModel)
        {
            var dataRow = base.CreateDataRow(eventModel);
            dataRow["ContractType"] = (byte)eventModel.ContractType;
            dataRow["AgreementId"] = eventModel.AgreementId;
            eventModel.Periods.ForEach(period => periods.Rows.Add(CreatePeriodDataRow(period)));
            eventModel.PriceEpisodes.ForEach(priceEpisode => priceEpisodes.Rows.Add(CreatePriceEpisodeDataRow(priceEpisode)));
            return dataRow;
        }

        private DataRow CreatePeriodDataRow(EarningEventPeriodModel eventModel)
        {
            var dataRow = periods.NewRow();
            dataRow["EarningEventId"] = eventModel.EarningEventId;
            dataRow["PriceEpisodeIdentifier"] = eventModel.PriceEpisodeIdentifier;
            dataRow["TransactionType"] = (byte)eventModel.TransactionType;
            dataRow["DeliveryPeriod"] = eventModel.DeliveryPeriod;
            dataRow["Amount"] = eventModel.Amount;
            return dataRow;
        }

        private DataRow CreatePriceEpisodeDataRow(EarningEventPriceEpisodeModel priceEpisodeModel)
        {
            var dataRow = periods.NewRow();
            dataRow["EarningEventId"] = priceEpisodeModel.EarningEventId;
            dataRow["PriceEpisodeIdentifier"] = priceEpisodeModel.PriceEpisodeIdentifier;
            dataRow["SfaContributionPercentage"] = priceEpisodeModel.SfaContributionPercentage;
            dataRow["StartDate"] = priceEpisodeModel.StartDate;
            dataRow["PlannedEndDate"] = priceEpisodeModel.PlannedEndDate;
            dataRow["ActualEndDate"] = priceEpisodeModel.ActualEndDate;
            dataRow["TotalNegotiatedPrice1"] = priceEpisodeModel.TotalNegotiatedPrice1;
            dataRow["TotalNegotiatedPrice2"] = priceEpisodeModel.TotalNegotiatedPrice2;
            dataRow["TotalNegotiatedPrice3"] = priceEpisodeModel.TotalNegotiatedPrice3;
            dataRow["TotalNegotiatedPrice4"] = priceEpisodeModel.TotalNegotiatedPrice4;
            dataRow["Completed"] = priceEpisodeModel.Completed;
            dataRow["CompletionAmount"] = priceEpisodeModel.CompletionAmount;
            dataRow["InstalmentAmount"] = priceEpisodeModel.InstalmentAmount;
            dataRow["NumberOfInstalments"] = priceEpisodeModel.NumberOfInstalments;
            return dataRow;

        }

        public override List<DataTable> GetDataTable(List<EarningEventModel> events)
        {
            periods.Rows.Clear();
            priceEpisodes.Rows.Clear();
            var tables = base.GetDataTable(events);
            tables.Add(priceEpisodes);
            tables.Add(periods);
            return tables;
        }
    }
}