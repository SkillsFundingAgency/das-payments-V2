using ClosedXML.Excel;
using Dapper;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using CommandLine;
using DocumentFormat.OpenXml.Spreadsheet;
using EarningsComparer.Filtering;
using Newtonsoft.Json;

namespace EarningsComparer
{
    class Program
    {
        internal const string DasQuery = "DasQuery.sql";
        internal const string DcQuery = "DcQuery.sql";
        internal const string ExcelTemplate = "Template.xlsx";
        internal const string BlackListFile = "blacklist.json";
        internal const string WhiteListFile = "whitelist.json";

        private static int Main(string[] args)
        {
            Parser argParser = new Parser(settings => settings.CaseInsensitiveEnumValues = true);

            return argParser.ParseArguments<Options>(args)
                .MapResult(RunAndReturnExitCode, _ => 1);
        }

        public static int RunAndReturnExitCode(Options options)
        {
            if (options.ProcessingStartTime.TimeOfDay == TimeSpan.Zero)
            {
                Console.WriteLine("Time component required on Processing Start Time");
                return 1;
            }

            try
            {
                CalculateEarningComparisonMetric(options.CollectionPeriod, options.ProcessingStartTime,
                    options.ProcessingFilterMode);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }

            return 0;
        }

        private static void CalculateEarningComparisonMetric(short collectionPeriod, DateTime processingStartTime,
            FilterMode filterMode)
        {
            var dasConnectionString = ConfigurationManager.ConnectionStrings["DasConnectionString"].ConnectionString;
            var dcConnectionString = ConfigurationManager.ConnectionStrings["DcConnectionString"].ConnectionString;
            var outputPath = ConfigurationManager.AppSettings["OutputPath"];

            var dasQuery = Helpers.ReadResource(DasQuery);
            var dcQuery = Helpers.ReadResource(DcQuery);

            IEnumerable<EarningsRow> dcData;
            IEnumerable<EarningsRow> dasData;

            using (var dcConnection = new SqlConnection(dcConnectionString))
            {
                dcData = dcConnection.Query<EarningsRow>(dcQuery,
                    new {collectionperiod = collectionPeriod},
                    commandTimeout: 5000);
            }

            using (var dasConnection = new SqlConnection(dasConnectionString))
            {
                dasData = dasConnection.Query<EarningsRow>(dasQuery,
                    new
                    {
                        collectionperiod = collectionPeriod,
                        monthendStartTime = processingStartTime
                    },
                    commandTimeout: 5000);
            }

            var joinedValues = dasData.FullJoin(
                    dcData,
                    earningsRow => new {earningsRow.Ukprn, earningsRow.ApprenticeshipContractType},
                    dasRow => new CombinedRow(dasRow.Ukprn, dasRow.ApprenticeshipContractType)
                        {DasRow = dasRow, DcRow = null},
                    dcRow => new CombinedRow(dcRow.Ukprn, dcRow.ApprenticeshipContractType)
                        {DasRow = null, DcRow = dcRow},
                    (dasRow, dcRow
                    ) => new CombinedRow(dasRow.Ukprn, dasRow.ApprenticeshipContractType)
                        {DasRow = dasRow, DcRow = dcRow}
                )
                .OrderBy(row => row.Ukprn)
                .ThenBy(row => row.ApprenticeshipContractType)
                .ToList();

            joinedValues =  FilterValues(filterMode, joinedValues);


            using (var templateStream = Helpers.OpenResource(ExcelTemplate))
            {
                using (var spreadsheet = new XLWorkbook(templateStream))
                {
                    var sheet = spreadsheet.Worksheet("Earnings Comparison");

                    AddSummaryInfo(sheet, collectionPeriod, processingStartTime);

                    WriteDataToSheet(sheet, joinedValues);


                    sheet.Columns().AdjustToContents();
                    sheet.Rows().AdjustToContents();

                    SaveWorksheet(spreadsheet, outputPath);
                }
            }
        }

        private static  List<CombinedRow> FilterValues(FilterMode filterMode, List<CombinedRow> joinedValues)
        {
            switch (filterMode)
            {
                case FilterMode.None:
                    return joinedValues;
                case FilterMode.Whitelist:
                    List<long> whiteList = CreateFilterList(filterMode);
                    return joinedValues.Where(jv => whiteList.Contains(jv.Ukprn)).ToList();
                   
                case FilterMode.Blacklist:
                    List<long> blackList = CreateFilterList(filterMode);
                    return  joinedValues.Where(jv => !blackList.Contains(jv.Ukprn)).ToList();
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterMode), filterMode, null);
            }
        }

        private static List<long> CreateFilterList(FilterMode filterMode)
        {
            return filterMode == FilterMode.Blacklist
                ? LoadFilterValues(BlackListFile)
                : LoadFilterValues(WhiteListFile);
        }

        private static List<long> LoadFilterValues(string jsonList)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                $@"Filtering\Files\{jsonList}");
            string filtersListText = File.ReadAllText(path);
            var filterList = JsonConvert.DeserializeObject<FilterList>(filtersListText);
            //var longList = Array.ConvertAll<string, long>(filterList.FilterValues.ToArray(), Int64.Parse).ToList();
            return filterList.FilterValues;
        }


        private static void AddSummaryInfo(IXLWorksheet sheet, in short collectionPeriod,
            in DateTime processingStartTime)
        {
            sheet.Cell(1, 1)
                .SetValue(
                    $"Report params: Collection Period:{collectionPeriod}, processingStartTime:{processingStartTime:G}");
        }

        private static void WriteDataToSheet(IXLWorksheet sheet, List<CombinedRow> test)
        {
            var row = 10;
            foreach (var combinedRow in test)
            {
                sheet.Cell(row, 1).SetValue(combinedRow.Ukprn);
                sheet.Cell(row, 2).SetValue(combinedRow.ApprenticeshipContractType);
                sheet.Cell(row, 3).SetValue(combinedRow.DcRow?.AllTypes ?? 0m);
                sheet.Cell(row, 4).SetValue(combinedRow.DasRow?.AllTypes ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 5).SetValue(combinedRow.TotalsEqual).SetRedIfFalse().SetGreenIfTrue();
                sheet.Cell(row, 6).SetValue(combinedRow.TotalsDifference);

                sheet.Cell(row, 7).SetValue(combinedRow.DcRow?.TT1 ?? 0m);
                sheet.Cell(row, 8).SetValue(combinedRow.DasRow?.TT1 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 9).SetValue(combinedRow.DcRow?.TT2 ?? 0m);
                sheet.Cell(row, 10).SetValue(combinedRow.DasRow?.TT2 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 11).SetValue(combinedRow.DasRow?.TT3 ?? 0m);
                sheet.Cell(row, 12).SetValue(combinedRow.DasRow?.TT3 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 13).SetValue(combinedRow.DasRow?.TT4 ?? 0m);
                sheet.Cell(row, 14).SetValue(combinedRow.DasRow?.TT4 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 15).SetValue(combinedRow.DasRow?.TT5 ?? 0m);
                sheet.Cell(row, 16).SetValue(combinedRow.DasRow?.TT5 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 17).SetValue(combinedRow.DasRow?.TT6 ?? 0m);
                sheet.Cell(row, 18).SetValue(combinedRow.DasRow?.TT6 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 19).SetValue(combinedRow.DasRow?.TT7 ?? 0m);
                sheet.Cell(row, 20).SetValue(combinedRow.DasRow?.TT7 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 21).SetValue(combinedRow.DasRow?.TT8 ?? 0m);
                sheet.Cell(row, 22).SetValue(combinedRow.DasRow?.TT8 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 23).SetValue(combinedRow.DasRow?.TT9 ?? 0m);
                sheet.Cell(row, 24).SetValue(combinedRow.DasRow?.TT9 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 25).SetValue(combinedRow.DasRow?.TT10 ?? 0m);
                sheet.Cell(row, 26).SetValue(combinedRow.DasRow?.TT10 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 27).SetValue(combinedRow.DcRow?.TT11 ?? 0m);
                sheet.Cell(row, 28).SetValue(combinedRow.DasRow?.TT11 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 29).SetValue(combinedRow.DcRow?.TT12 ?? 0m);
                sheet.Cell(row, 30).SetValue(combinedRow.DasRow?.TT12 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 31).SetValue(combinedRow.DasRow?.TT13 ?? 0m);
                sheet.Cell(row, 32).SetValue(combinedRow.DasRow?.TT13 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 33).SetValue(combinedRow.DasRow?.TT14 ?? 0m);
                sheet.Cell(row, 34).SetValue(combinedRow.DasRow?.TT14 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 35).SetValue(combinedRow.DasRow?.TT15 ?? 0m);
                sheet.Cell(row, 36).SetValue(combinedRow.DasRow?.TT15 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                sheet.Cell(row, 37).SetValue(combinedRow.DasRow?.TT16 ?? 0m);
                sheet.Cell(row, 38).SetValue(combinedRow.DasRow?.TT16 ?? 0m)
                    .SetRedIfNotEqualToPrevious()
                    .SetGreenIfEqualToPrevious();

                row++;
            }
        }

        private static void SetConditionalFormatting(IXLCell cell)
        {
            cell.CellLeft(1);
            var formula = $"={cell.Address}<>{cell.CellLeft(1).Address}";
            cell.AddConditionalFormat().WhenIsTrue(formula)
                .Fill.SetBackgroundColor(XLColor.FromHtml("#FFC1B4"));
        }


        private static void SaveWorksheet(XLWorkbook workbook, string path)
        {
            workbook.CalculationOnSave = true;
            var date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");

            var filename = $"{path}\\EarningsComparisonReport-{date}.xlsx";
            Console.WriteLine($"Saving to: {filename}");
            workbook.SaveAs(filename);
        }
    }
}