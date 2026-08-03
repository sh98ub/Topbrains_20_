using System;
using NUnit.Framework;

namespace Q41_NUnitBankAccount
{
    [TestFixture]
    public class UnitTest
    {
        [Test]
        public void Test_Deposit_ValidAmount()
        {
            Program account = new Program(100m);
            account.Deposit(50m);
            Assert.That(account.Balance, Is.EqualTo(150m));
        }

        [Test]
        public void Test_Deposit_NegativeAmount()
        {
            Program account = new Program(100m);
            Assert.That(Assert.Throws<ArgumentException>(() => account.Deposit(-50m))!.Message, Is.EqualTo("Deposit amount cannot be negative"));
        }

        [Test]
        public void Test_Withdraw_ValidAmount()
        {
            Program account = new Program(100m);
            account.Withdraw(40m);
            Assert.That(account.Balance, Is.EqualTo(60m));
        }

        [Test]
        public void Test_Withdraw_InsufficientFunds()
        {
            Program account = new Program(100m);
            Assert.That(Assert.Throws<InvalidOperationException>(() => account.Withdraw(200m))!.Message, Is.EqualTo("Insufficient funds."));
        }
    }
}
