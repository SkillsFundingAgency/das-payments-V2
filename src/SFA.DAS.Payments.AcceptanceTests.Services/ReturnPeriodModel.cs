namespace SFA.DAS.Payments.AcceptanceTests.Services
{
    using System.Globalization;

    public class ReturnPeriodModel
    {
        private readonly string _periodName;

        public ReturnPeriodModel(int periodNumber)
        {
            PeriodNumber = periodNumber;
            _periodName = $"R{periodNumber.ToString("00", NumberFormatInfo.InvariantInfo)}";
        }

        public int PeriodNumber { get; set; }

        public string PeriodName() => _periodName;

        public string NextOpeningDate { get; set; }
    }
}
