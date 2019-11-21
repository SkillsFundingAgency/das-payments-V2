using System.Text.RegularExpressions;

namespace SFA.DAS.Payments.Core
{
    public static class TypeString
    {
        public static bool TryParseTypeName(string typesString, out string typeName)
        {
            // e.g. "Some.Namespace.Class, Some.OtherNamespace.Interface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"
            // Extract "Class"
            var match = new Regex(@"(\w+),").Match(typesString);

            if (match.Groups.Count < 1)
            {
                typeName = null;
                return false;
            }
            else
            {
                typeName = match.Groups[1].Value;
                return true;
            }
        }
    }
}
