using System.Diagnostics;

namespace SFA.DAS.Payments.Model.Core
{
    [DebuggerDisplay("Collection: {Name}")]
    public class CollectionPeriod 
    {
        public short AcademicYear { get; set; }
        public string Name { get; set; }
        public byte Period { get; set; }

        public CollectionPeriod Clone()
        {
            return (CollectionPeriod) MemberwiseClone();
        }
    }
}