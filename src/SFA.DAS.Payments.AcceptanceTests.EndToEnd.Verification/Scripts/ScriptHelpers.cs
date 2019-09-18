using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Scripts
{
    public static class ScriptHelpers
    {

        public static string GetSqlScriptText(string file)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Scripts.{file}";

            using (var stream = assembly.GetManifestResourceStream(
                    resourceName))
            using (var reader = new StreamReader(stream))
            {
                string text = reader.ReadToEnd();
                return text;
            }

        }

    }
}
