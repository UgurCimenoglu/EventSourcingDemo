using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.Domain.BankCustomer;
using Bank.Domain.BankMoney;

namespace Bank.Domain.BankAccount
{
    public class Account
    {
        public long Version { get; private set; } = -1;
        public AccountId Id { get; set; }
        public AccountState State { get; } = new();

        public readonly List<object> _changes = new();

        public Account(AccountId id)
        {
            Id = id;
        }

        public Account(AccountId id, AccountSnapshot snapshot)
        {
            Id = id;
            State = snapshot.State;
            Version = snapshot.Version;

        }

        public IReadOnlyList<object> GetChanges() => _changes.AsReadOnly().ToArray();


        public void CreateAccount(CustomerId customerId, Currency currency)
        {
            AddEvent(new AccountCreated(customerId, currency));
        }

        public void DepositMoney(Money money)
        {
            AddEvent(new MoneyDeposited(money));
        }

        public void WithdrawMoney(Money money)
        {
            AddEvent(new MoneyWithdrawn(money));
        }

        private void AddEvent(object @event)
        {
            ApplyEvent(@event);
            _changes.Add(@event);
        }

        private void ApplyEvent(object @event)
        {
            switch (@event)
            {
                case AccountCreated accountCreated:
                    Apply(accountCreated);
                    break;

                case MoneyDeposited moneyDeposited:
                    Apply(moneyDeposited);
                    break;

                case MoneyWithdrawn moneyWithdrawn:
                    Apply(moneyWithdrawn);
                    break;
            }
        }

        private void Apply(MoneyWithdrawn moneyWithdrawn)
        {
            State.Balance -= moneyWithdrawn.Money;
        }

        private void Apply(MoneyDeposited moneyDeposited)
        {
            State.Balance += moneyDeposited.Money;
        }

        private void Apply(AccountCreated accountCreated)
        {
            State.CustomerId = accountCreated.CustomerId;
            State.Balance = Money.Zero(accountCreated.Currency);
        }

        public void LoadChanges(long version, object change)
        {
            Version = version;
            AddEvent(change);
        }
    }
}
