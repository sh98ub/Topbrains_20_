using System;

namespace Q41_NUnitBankAccount
{
    public class Program
    {
        public decimal Balance { get; set; }

        public Program(decimal initialBalance)
        {
            Balance = initialBalance;
        }

        public void Deposit(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Deposit amount cannot be negative");
            }
            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount > Balance)
            {
                throw new InvalidOperationException("Insufficient funds.");
            }
            if (amount < 0)
            {
                throw new ArgumentException("Withdrawal amount cannot be negative");
            }
            Balance -= amount;
        }
    }
}
