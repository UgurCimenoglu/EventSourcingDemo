using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.BankMoney
{
    public record Currency
    {
        public string Name { get; set; }
        public string Symbol { get; set; }

        public Currency(string name, string symbol)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(symbol);

            Name = name;
            Symbol = symbol;
        }

        public static Currency TL = GetByName("TL");
        public static Currency USD = GetByName("USD");

        private static Currency GetByName(string name)
        {
            return name switch
            {
                "TL" => new Currency("TL", "₺"),
                "USD" => new Currency("USD", "$")
            };
        }

        public override string ToString() => Name;

        public static implicit operator string(Currency currency) => currency.ToString();
    }
}
