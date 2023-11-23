using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.BankMoney
{
    public class CurrencyIsNotValid : Exception
    {
        public CurrencyIsNotValid()  : base("The currency is not valid.") { }
    }
}
