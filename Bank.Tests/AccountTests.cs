using Bank.Domain.BankAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.Domain.BankCustomer;
using Bank.Domain.BankMoney;

namespace Bank.Tests
{
    public class AccountTests
    {
        [Fact]
        public void Should_Be_True_When_Add_Events()
        {
            var account = new Account(new AccountId(new Guid()));
            account.CreateAccount(new CustomerId(new Guid()), Currency.TL);
            account.DepositMoney(new Money(500M, Currency.TL));
            account.WithdrawMoney(new Money(250M, Currency.TL));

            Assert.Equal(new Money(250M, Currency.TL), account.State.Balance);
            Assert.Equal(3, account.GetChanges().Count);
            Assert.Collection(account.GetChanges(),
                e => Assert.IsType<AccountCreated>(e),
                e => Assert.IsType<MoneyDeposited>(e),
                e => Assert.IsType<MoneyWithdrawn>(e)
             );
        }

        [Fact]
        public void Should_Be_True_When_LoadChanges()
        {
            var account = new Account(new AccountId(new Guid()));

            account.LoadChanges(0, new AccountCreated(new CustomerId(new Guid()), Currency.TL));
            account.LoadChanges(1, new MoneyDeposited(new Money(500M, Currency.TL)));
            account.LoadChanges(2, new MoneyWithdrawn(new Money(250M, Currency.TL)));

            Assert.Equal(new Money(250M, Currency.TL), account.State.Balance);
            Assert.Equal(2, account.Version);
        }

        [Fact]
        public void Should_Be_True_When_LoadState()
        {
            var account = new Account(
                new AccountId(new Guid()),
                new AccountSnapshot
                {
                    State = new AccountState { Balance = new Money(250, Currency.TL), CustomerId = new CustomerId(new Guid()) },
                    Version = 2
                });

            account.WithdrawMoney(new Money(250, Currency.TL));

            Assert.Equal(Money.Zero(Currency.TL), account.State.Balance);
            Assert.Single(account.GetChanges());
            Assert.Collection(account.GetChanges(),
                e =>
            {
                Assert.IsType<MoneyWithdrawn>(e);
            });

        }
    }
}
