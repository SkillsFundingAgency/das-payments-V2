using System;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class CollectionPeriodModel
    {
        public long Id { get; set; }
        public short AcademicYear { get; set; }
        public byte Period { get; set; }
        public byte CalendarMonth { get; set; }
        public short CalendarYear { get; set; }
        public DateTime ReferenceDataValidationDate { get; set; }
        public DateTime CompletionDate { get; set; }
    }
}