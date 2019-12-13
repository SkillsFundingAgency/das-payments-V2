using System.IO;
using System.Reflection;
using ClosedXML.Excel;

namespace EarningsComparer
{
    internal static class Helpers
    {
        internal static Stream OpenResource(string filename)
        {
            var assembly = Assembly.GetEntryAssembly();

            var stream = assembly
                .GetManifestResourceStream($"{assembly.GetName().Name}.Resources.{filename}");
            return stream;
        }


        internal static string ReadResource(string filename)
        {
            var stream = OpenResource(filename);
            string text;
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            return text;
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