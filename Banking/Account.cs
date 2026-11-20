
using BankingApplicatione.Banking;

namespace BankingApplicatione.Banking;
public class TransactionLog
{
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = "";
    public IBankingAccount<TransactionLog>? From { get; set; } = null;
    public double Amount { get; set; }
}

class BasicAuth : AuthenticationManager<string>
{
    public required string Username { get; set; }
    public required string HashedPassword { get; set; }
}

class SavingsAccount : IBankingAccount<TransactionLog>
{
    private bool _closed = true;
    private double _balance = 0.0;
    private List<TransactionLog> _history = new List<TransactionLog>();
    public string? Name { get; set;}
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public Type? BankingProvider { get; set; }
    public DateTime CreationDate { get; init; }
    public DateTime? TerminatedAt { get; set; }
    public bool IsClosed { get => _closed; }

    public short Close()
    {
        if (_closed || TerminatedAt is not null) return StatusFactory.AccountAlreadyClosed(); else
        {
            _closed = true;
            return StatusFactory.Success();
        }
    }

    public short CreditWindow(double accuracy)
    {
        throw new NotImplementedException();
    }


    private string SerializeNumber(double amount)
    {
        string[] suffixes = { "", "K", "M", "B", "T", "Qa" };

        if (amount == 0)
            return "0";

        string sign = amount < 0 ? "-" : "";
        amount = Math.Abs(amount);

        List<string> parts = new();

        for (int i = suffixes.Length - 1; i >= 0; i--)
        {
            double unit = Math.Pow(1000, i);

            if (amount >= unit)
            {
                double value = Math.Floor(amount / unit);
                amount %= unit;

                parts.Add($"{value:0}{suffixes[i]}");
            }
        }

        if (amount > 0)
            parts.Add($"{amount:0.####}");

        return sign + string.Join(" + ", parts);
    }


    private void AppendToPublicRecord(double amount, IBankingAccount<TransactionLog>? account, string action)
    {
        _history.Add(
            new TransactionLog
            {
                Timestamp = DateTime.Now,
                Action = $"{action}: To '{account?.Name ?? "Master_Account"}' Amount: '{SerializeNumber(amount)}'",
                From = account,
                Amount = amount
            }
        );
    }

    public short Deposit(double amount, IBankingAccount<TransactionLog>? account)
    {
        AppendToPublicRecord(amount, account, "Deposit");
        if (account is null)
        {
            _balance -= amount;
            return StatusFactory.DepositFromMasterAccountSuccessful();
        } else
        {
            if (_balance >= amount)
            {
                _balance -= amount;
                account.Withdraw(amount, null);
                return StatusFactory.DepositSuccessful();
            }
            return StatusFactory.NotEnoughMoney();
        }
    }

    public double GetBalance()
    {
        return Math.Max(0, _balance); // Ensure balance is never negative
    }

    public double GetDebt()
    {
        return Math.Abs(Math.Min(0, _balance));
    }

    public IReadOnlyList<TransactionLog> GetHistory()
    {
        return _history;
    }

    public double GetTotal()
    {
        return _balance;
    }

    public short Open()
    {
        if (!_closed || TerminatedAt is not null) return StatusFactory.AccountAlreadyOpenedOrTerminated();
        else
        {
            _closed = false;
            return StatusFactory.Success();
        }
    }

    public short Terminate(DateTime? time, string reason, short statusCode)
    {
        var pubTime = time ?? DateTime.Now;


        _history.Add(new TransactionLog
        {
            Timestamp = pubTime,
            Action = $"Account terminated: {reason}, Status Code: {statusCode}",
            From = null,
            Amount = 0
        });

        TerminatedAt = pubTime;

        return StatusFactory.Success();
    }

    public short Withdraw(double amount, IBankingAccount<TransactionLog>? account)
    {
        AppendToPublicRecord(amount, account, "Withdraw");
        if (account is null) {
            _balance += amount;
            return StatusFactory.WithdrawFromMasterAccountSuccessful();
        } else {
            if (account.GetBalance() >= amount) {
                account.Deposit(amount, null);
                _balance += amount;
                return StatusFactory.WithdrawSuccessfull();
            }

            return StatusFactory.NotEnoughMoney();
        }
    }

    public short WithdrawDebt(double amount, string reason)
    {
        AppendToPublicRecord(amount, null, "Debt_Withdrawal");
        _balance -= amount;

        return StatusFactory.WithdrawSuccessfull();
    }

    public short DepositDebt(double amount, string reason)
    {
        AppendToPublicRecord(amount, null, "Debt_Deposit");
        _balance += amount;

        return StatusFactory.DepositSuccessful();
    }
}


class SavingsContainer : AccountContainer<SavingsAccount, uint, TransactionLog>
{
    public SavingsAccount account { get; set; }

    public uint integral { get; }
    
    public SavingsContainer(SavingsAccount account, uint id)
    {
        this.account = account;
        integral = id;
    }
}