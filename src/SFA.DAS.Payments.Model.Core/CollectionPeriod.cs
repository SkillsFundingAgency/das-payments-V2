using System.Diagnostics;

namespace SFA.DAS.Payments.Model.Core
{
    [DebuggerDisplay("Collection: {AcademicYear}-R{Period}")]
    public class CollectionPeriod 
    {
        public short AcademicYear { get; set; }
        public byte Period { get; set; }
        public CollectionPeriod Clone()
        {
            return (CollectionPeriod) MemberwiseClone();
        }
    }
}