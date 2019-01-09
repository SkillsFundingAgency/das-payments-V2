namespace SFA.DAS.Payments.Model.Core
{
    public abstract class SfaPeriodBaseClass
    {
        public short Year { get; set; }
        public byte Month { get; set; }
        private byte period = 0;
        public byte Period
        {
            get
            {
                if (period == 0)
                {
                    if (Month < 8)
                    {
                        period = (byte)(Month + 5);
                    }
                    else
                    {
                        period = (byte)(Month - 7);
                    }
                }

                return period;
            }
            set => period = value;
        }
    }
}