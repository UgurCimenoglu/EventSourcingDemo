using Bank.Domain.BankCustomer;
using Bank.Domain.BankMoney;

namespace Bank.Domain.BankAccount;

public record AccountCreated(CustomerId CustomerId, Currency Currency);