using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.BankAccount
{
    public class AccountId
    {
        public Guid Id { get; set; }

        public AccountId(Guid id)
        {
            Id = id;
        }

        public override string ToString() => Id.ToString();
    }
}
