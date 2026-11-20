using BankingApplicatione.Banking.Cryptography;

namespace BankingApplicatione.Banking;


class Banking : IBankSystem<SavingsAccount, TransactionLog>
{
    private List<SavingsAccount> _accounts = new();
    private uint curId = 0;
    public AccountContainer<SavingsAccount, uint, TransactionLog> AddAccount(SavingsAccount account)
    {
        account.BankingProvider = typeof(Banking);
        _accounts.Add(account);
        return new SavingsContainer(account, curId++);
    }

    public bool HasAccount(SavingsAccount account)
    {
        return _accounts.Contains(account);
    }

    public short RemoveAccount(SavingsAccount account)
    {
        _accounts.Remove(account);
        return StatusFactory.Success();
    }
}