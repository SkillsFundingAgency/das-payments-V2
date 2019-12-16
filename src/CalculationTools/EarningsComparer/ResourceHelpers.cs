using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ClosedXML.Excel;
using EarningsComparer.Filtering;
using Newtonsoft.Json;

namespace EarningsComparer
{
    internal static class ResourceHelpers
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

        internal static List<long> LoadFilterValues(string jsonList)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                $@"Filtering\Files\{jsonList}");
            string filtersListText = File.ReadAllText(path);
            var filterList = JsonConvert.DeserializeObject<FilterList>(filtersListText);
            return filterList.FilterValues;
        }


    }
}