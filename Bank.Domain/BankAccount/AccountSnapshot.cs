using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.BankAccount
{
    public class AccountSnapshot
    {
        public AccountState State { get; set; }
        public long Version { get; set; }
    }
}
