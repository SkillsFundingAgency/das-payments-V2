using System.Diagnostics;

namespace SFA.DAS.Payments.Model.Core
{
    [DebuggerDisplay("Collection: {AcademicYear}-R{Period}")]
    public class CollectionPeriod 
    {
        public short AcademicYear { get; set; }
        //public string Name => $"{AcademicYear}-R{Period:00}";  //TODO: Move this property out of this class.  Only tests use this property so should not pollute the services.
        public byte Period { get; set; }
    }
}