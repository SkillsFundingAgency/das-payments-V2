namespace SFA.DAS.Payments.AcceptanceTests.Core.Services
{
    using System;
    using DCT.TestDataGenerator;

    public class UlnService : IUlnService
    {
        public long GenerateUln(long index)
        {
            try
            {
                return ListOfULNs.ULN(index);
            }
            catch (ArgumentOutOfRangeException)
            {
                return ListOfULNs.ULN(index + 1);
            }
        }
    }
}