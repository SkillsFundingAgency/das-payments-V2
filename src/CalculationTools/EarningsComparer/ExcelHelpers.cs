using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;

namespace EarningsComparer
{
    internal static class ExcelHelpers
    {

        internal static void AddRowData<T>(this IXLWorksheet sheet, T filterItems, int row = 1, int column = 1) where T : IList
        { 
            foreach (var filterItem in filterItems)
            {
                sheet.Cell(row, column).SetValue(filterItem);
                row++;
            }
        }

        internal static void SetConditionalFormatting(IXLCell cell)
        {
            cell.CellLeft(1);
            var formula = $"={cell.Address}<>{cell.CellLeft(1).Address}";
            cell.AddConditionalFormat().WhenIsTrue(formula)
                .Fill.SetBackgroundColor(XLColor.FromHtml("#FFC1B4"));
        }

        internal static void AdjustToContent(this IXLWorksheet sheet)
        {
            sheet.Columns().AdjustToContents();
            sheet.Rows().AdjustToContents();
        }

        internal static void SetAsTable(this IXLWorksheet sheet, int startRow, int startCol)
        {
            var range = sheet.Range(sheet.Cell(startRow, startCol), sheet.LastCellUsed());
            range.CreateTable("EarningsTable");
        }


        internal static void SaveWorksheet(XLWorkbook workbook, string path)
        {
            workbook.CalculationOnSave = true;
            var date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");

            var filename = $"{path}\\EarningsComparisonReport-{date}.xlsx";
            Console.WriteLine($"Saving to: {filename}");
            workbook.SaveAs(filename);
        }

        public static IXLCell SetAsPercentage(this IXLCell cell)
        {
            cell.Style.NumberFormat.NumberFormatId = 10;

            return cell;
        }



        
        public static IXLCell SetRedIfFalse(this IXLCell cell)
        {
            cell.CellLeft(1);
            var formula =  $"={cell.Address}=False";
            cell.AddConditionalFormat().WhenIsTrue(formula)
                .Fill.SetBackgroundColor(XLColor.FromHtml("#FFC1B4"));
            return cell;
        }

        public static IXLCell SetGreenIfTrue(this IXLCell cell)
        {
            cell.CellLeft(1);
            var formula =  $"={cell.Address}=True";
            cell.AddConditionalFormat().WhenIsTrue(formula)
                .Fill.SetBackgroundColor(XLColor.FromHtml("#B4FFC8"));
            return cell;
        }


        public static IXLCell SetRedIfNotEqualToPrevious(this IXLCell cell)
        {
            cell.CellLeft(1);
            var formula =  $"={cell.Address}<>{cell.CellLeft(1).Address}";
            cell.AddConditionalFormat().WhenIsTrue(formula)
                .Fill.SetBackgroundColor(XLColor.FromHtml("#FFC1B4"));
            return cell;
        }

        public static IXLCell SetGreenIfEqualToPrevious(this IXLCell cell)
        {
            cell.CellLeft(1);
            var formula =  $"={cell.Address}={cell.CellLeft(1).Address}";
            cell.AddConditionalFormat().WhenIsTrue(formula)
                .Fill.SetBackgroundColor(XLColor.FromHtml("#B4FFC8"));
            return cell;
        }
    }
}
