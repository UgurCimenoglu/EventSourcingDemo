using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.BankCustomer
{
    public class CustomerId
    {
        public Guid Id { get; set; }

        public CustomerId(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            Id = id;
        }
    }
}
