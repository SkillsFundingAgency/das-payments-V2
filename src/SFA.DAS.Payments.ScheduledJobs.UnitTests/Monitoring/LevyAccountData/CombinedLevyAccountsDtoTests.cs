using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.ScheduledJobs.Monitoring.LevyAccountData;

namespace SFA.DAS.Payments.ScheduledJobs.UnitTests.Monitoring.LevyAccountData
{
    [TestFixture]
    public class CombinedLevyAccountsDtoTests
    {
        private readonly LevyAccountBuilder levyAccountBuilder = new LevyAccountBuilder();
        
        [TestCase(2, 2, 200, 200)]
        [TestCase(1, 2, 100, 200)]
        [TestCase(2, 1, 200, 100)]
        [TestCase(0, 1, 0, 100)]
        [TestCase(-1, 1, 0, 100)]
        [TestCase(1, 0, 100, 0)]
        [TestCase(1, -1, 100, 0)]
        public void GetLevyAccounts_Should_Calculate_Totals_From_Das_And_Payments_LevyAccount_Lists(int numberOfItemsDas, int numberOfItemsPayments, int expectedDasSum, int expectedPaymentsSum)
        {
            var sut = new CombinedLevyAccountsDto(levyAccountBuilder.Build(numberOfItemsDas), levyAccountBuilder.Build(numberOfItemsPayments));

            numberOfItemsDas = numberOfItemsDas == -1 ? 0 : numberOfItemsDas;
            numberOfItemsPayments = numberOfItemsPayments == -1 ? 0 : numberOfItemsPayments;
            
            sut.DasLevyAccountCount.Should().Be(numberOfItemsDas);
            sut.PaymentsLevyAccountCount.Should().Be(numberOfItemsPayments);
            
            sut.DasLevyAccountBalanceTotal.Should().Be(expectedDasSum);
            sut.PaymentsLevyAccountBalanceTotal.Should().Be(expectedPaymentsSum);
            
            sut.DasTransferAllowanceTotal.Should().Be(expectedDasSum);
            sut.PaymentsTransferAllowanceTotal.Should().Be(expectedPaymentsSum);
            
            sut.DasIsLevyPayerCount.Should().Be(numberOfItemsDas);
            sut.PaymentsIsLevyPayerCount.Should().Be(numberOfItemsPayments);
        }

        [Test]
        public void GetLevyAccounts_Should_Returned_Joined_LevyAccounts_From_Das_And_Payments_LevyAccount_Lists()
        {
            var sut = new CombinedLevyAccountsDto(levyAccountBuilder.Build(1), levyAccountBuilder.Build(1));

            var levyAccountsDtos = sut.LevyAccounts.ToList();

            levyAccountsDtos.Count.Should().Be(1);

            levyAccountsDtos.ElementAt(0).DasLevyAccount.Should().NotBeNull();
            levyAccountsDtos.ElementAt(0).PaymentsLevyAccount.Should().NotBeNull();
            
            levyAccountsDtos.ElementAt(0).DasLevyAccount.AccountId.Should().Be(1);
            levyAccountsDtos.ElementAt(0).PaymentsLevyAccount.AccountId.Should().Be(1);
        }

        [Test]
        public void GetLevyAccounts_Should_Return_Default_Das_LevyAccount_When_Payments_LevyAccount_Does_Not_Have_Matching_Entity()
        {
            var sut = new CombinedLevyAccountsDto(levyAccountBuilder.Build(2), levyAccountBuilder.Build(1));

            var levyAccountsDtos = sut.LevyAccounts.ToList();

            levyAccountsDtos.Should().HaveCount(2);

            levyAccountsDtos.ElementAt(0).DasLevyAccount.Should().NotBeNull();
            levyAccountsDtos.ElementAt(0).PaymentsLevyAccount.Should().NotBeNull();
            
            levyAccountsDtos.ElementAt(0).DasLevyAccount.AccountId.Should().Be(1);
            levyAccountsDtos.ElementAt(0).PaymentsLevyAccount.AccountId.Should().Be(1);

            levyAccountsDtos.ElementAt(1).DasLevyAccount.Should().NotBeNull();
            levyAccountsDtos.ElementAt(1).PaymentsLevyAccount.Should().NotBeNull();
            
            levyAccountsDtos.ElementAt(1).DasLevyAccount.AccountId.Should().Be(2);
            
            levyAccountsDtos.ElementAt(1).PaymentsLevyAccount.AccountId.Should().Be(0);
            levyAccountsDtos.ElementAt(1).PaymentsLevyAccount.AccountName.Should().BeNull();
        }

        [Test]
        public void GetLevyAccounts_Should_Return_Default_Payments_LevyAccount_When_Das_LevyAccount_Does_Not_Have_Matching_Entity()
        {
            var sut = new CombinedLevyAccountsDto(levyAccountBuilder.Build(1), levyAccountBuilder.Build(2));

            var levyAccountsDtos = sut.LevyAccounts.ToList();

            levyAccountsDtos.Should().HaveCount(2);

            levyAccountsDtos.ElementAt(0).DasLevyAccount.Should().NotBeNull();
            levyAccountsDtos.ElementAt(0).PaymentsLevyAccount.Should().NotBeNull();
            
            levyAccountsDtos.ElementAt(0).DasLevyAccount.AccountId.Should().Be(1);
            levyAccountsDtos.ElementAt(0).PaymentsLevyAccount.AccountId.Should().Be(1);

            levyAccountsDtos.ElementAt(1).DasLevyAccount.Should().NotBeNull();
            levyAccountsDtos.ElementAt(1).PaymentsLevyAccount.Should().NotBeNull();
            
            levyAccountsDtos.ElementAt(1).PaymentsLevyAccount.AccountId.Should().Be(2);
            
            levyAccountsDtos.ElementAt(1).DasLevyAccount.AccountId.Should().Be(0);
            levyAccountsDtos.ElementAt(1).DasLevyAccount.AccountName.Should().BeNull();
        }

        [Test]
        public void GetLevyAccounts_Should_Return_OuterJoined_LevyAccount_List_When_Das_Or_Payments_List_Does_Not_Have_Matching_Entity()
        {
            var accounts = levyAccountBuilder.Build(3).ToList();
            
            var sut = new CombinedLevyAccountsDto(accounts.Skip(1), accounts.Take(2));

            var levyAccountsDtos = sut.LevyAccounts.ToList();

            levyAccountsDtos.Count.Should().Be(3);

            //Account 1
            bool Account1(LevyAccountsDto d) => d.DasLevyAccount.AccountId == 0;
            levyAccountsDtos.Single(Account1).DasLevyAccount.Should().NotBeNull();
            levyAccountsDtos.Single(Account1).PaymentsLevyAccount.Should().NotBeNull();
            
            levyAccountsDtos.Single(Account1).DasLevyAccount.AccountName.Should().BeNull();
            levyAccountsDtos.Single(Account1).PaymentsLevyAccount.AccountName.Should().Be("AccountName");
            levyAccountsDtos.Single(Account1).DasLevyAccount.AccountId.Should().Be(0);
            levyAccountsDtos.Single(Account1).PaymentsLevyAccount.AccountId.Should().Be(1);

            //Account 2
            bool Account2(LevyAccountsDto d) => d.DasLevyAccount.AccountId == 2;
            levyAccountsDtos.Single(Account2).DasLevyAccount.Should().NotBeNull();
            levyAccountsDtos.Single(Account2).PaymentsLevyAccount.Should().NotBeNull();
            
            levyAccountsDtos.Single(Account2).DasLevyAccount.AccountName.Should().Be("AccountName");
            levyAccountsDtos.Single(Account2).PaymentsLevyAccount.AccountName.Should().Be("AccountName");
            levyAccountsDtos.Single(Account2).DasLevyAccount.AccountId.Should().Be(2);
            levyAccountsDtos.Single(Account2).PaymentsLevyAccount.AccountId.Should().Be(2);

            //Account 3
            bool Account3(LevyAccountsDto d) => d.DasLevyAccount.AccountId == 3;
            levyAccountsDtos.Single(Account3).DasLevyAccount.Should().NotBeNull();
            levyAccountsDtos.Single(Account3).PaymentsLevyAccount.Should().NotBeNull();
            
            levyAccountsDtos.Single(Account3).DasLevyAccount.AccountName.Should().Be("AccountName");
            levyAccountsDtos.Single(Account3).PaymentsLevyAccount.AccountName.Should().BeNull();
            levyAccountsDtos.Single(Account3).DasLevyAccount.AccountId.Should().Be(3);
            levyAccountsDtos.Single(Account3).PaymentsLevyAccount.AccountId.Should().Be(0);
        }
    }
}
