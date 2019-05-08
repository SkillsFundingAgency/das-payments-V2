namespace SFA.DAS.Payments.AcceptanceTests.Core.Services
{
    public class UlnService : IUlnService
    {
        public long GenerateUln(long index) => DCT.TestDataGenerator.ListOfULNs.ULN(index);
    }
}