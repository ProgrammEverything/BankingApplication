namespace BankingApplicatione.Banking;

// Using aliases to keep your domain logic clean
using Money = System.Double;
using Status = System.Int16;

class StatusFactory
{
    public static Status Success() => 0;
    public static Status Failure() => 10;
    public static Status NotDefined() => 212;
    public static Status NotEnoughCredit() => 42;
    public static Status ResultShownSuccessfully() => 23;
    public static Status ResultNotShownSuccessfully() => 23;

    public static Status AccountAlreadyClosed() => 100;
    public static Status AccountAlreadyOpenedOrTerminated() => 101;

    public static Status DepositSuccessful() => 200;
    public static Status DepositFromMasterAccountSuccessful() => 201;

    public static Status NotEnoughMoney() => 202;

    public static Status WithdrawSuccessfull() => 210;
    public static Status WithdrawFromMasterAccountSuccessful() => 211;


}

/// <summary>
/// Handles the core movement of money. 
/// This is the "Engine" of the account.
/// </summary>
public interface ITransactionEngine<TMetadata>
{
    Status Deposit(Money amount, IBankingAccount<TMetadata>? account);
    Status Withdraw(Money amount, IBankingAccount<TMetadata>? account);
    Status CreditWindow(Money accuracy);

    // Fixed your List<object> problem using Generics!
    // Now you can have List<TransactionInfo> instead of just 'object'
    IReadOnlyList<TMetadata> GetHistory();

    Money GetBalance();
    Money GetDebt();
    Status WithdrawDebt(Money amount, string reason);
    Status DepositDebt(Money amount, string reason);
    Money GetTotal();
}

/// <summary>
/// Handles the "Who" and "When".
/// </summary>
public interface IAccountIdentity
{
    string? Name { get; set; }
    string? LastName { get; set; }
    string? PhoneNumber { get; set; }
    string? Email { get; set; }
    Type? BankingProvider { get; set; }
    DateTime CreationDate { get; init; } // 'init' means it can only be set during creation
    DateTime? TerminatedAt { get; set; }
}

/// <summary>
/// Handles the "Life" of the account.
/// </summary>
public interface IAccountLifecycle
{
    bool IsClosed { get; }
    Status Open();
    Status Close();
    Status Terminate(DateTime? time, string reason, Status statusCode);
}

/// <summary>
/// The "Master" interface that combines everything.
/// A class implements this to be a "Full" Bank Account.
/// </summary>
/// 
public interface GeneralAccount<TMetadata> : 
    IAccountIdentity,
    ITransactionEngine<TMetadata>
{

}

public interface IBankingAccount<TMetadata> :
    IAccountIdentity,
    ITransactionEngine<TMetadata>,
    IAccountLifecycle
{
    // This can stay empty; it just acts as a "Bundle" of the others.
}

public interface AccountContainer<AccountType, Integral, T> where 
    AccountType : class, IBankingAccount<T>
{
    public AccountType account { get; set; }
    public Integral integral { get; }

}

/// <summary>
/// The Bank itself (The Orchestrator).
/// </summary>
public interface IBankSystem<TAccount, T> where TAccount : class, IBankingAccount<T>
{
    AccountContainer<TAccount, uint, T> AddAccount(TAccount account);
    Status RemoveAccount(TAccount account);
    bool HasAccount(TAccount account);
}

public interface AuthenticationManager<HashType>
{
    string Username { get; set; }

    HashType HashedPassword { get; set; }
}