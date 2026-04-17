using Financii.Domain.Contracts;
using Financii.Domain.Enums;

namespace Financii.Domain.Entities
{
    public class BankAccount : EntityBase
    {
        protected BankAccount() { }

        public BankAccount(
            string name,
            string bankName,
            AccountType type,
            decimal currentBalance,
            string currency)
        {
            PublicId = Guid.NewGuid();
            Name = name;
            BankName = bankName;
            Type = type;
            CurrentBalance = currentBalance;
            Currency = currency;
        }

        public string Name { get; private set; } = string.Empty;
        public string BankName { get; private set; } = string.Empty;
        public AccountType Type { get; private set; }
        public decimal CurrentBalance { get; private set; }
        public string Currency { get; private set; } = string.Empty;
    }
}
