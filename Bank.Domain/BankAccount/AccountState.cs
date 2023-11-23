using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.Domain.BankCustomer;
using Bank.Domain.BankMoney;

namespace Bank.Domain.BankAccount
{
    public class AccountState
    {
        public CustomerId CustomerId { get; set; }
        public Money  Balance { get; set; }
    }
}
