using BankingApplicatione.Banking;
using BankingApplicatione.Banking.Cryptography;

Banking normalBank = new();

SavingsAccount generalAccount = new()
{
    Name = "SumUser",
    LastName = "OtherUser",
    CreationDate = DateTime.Now,
    Email = "sampleUser@gmail.com",
    PhoneNumber = "+989111111111"
};

SavingsAccount anotherAccount = new()
{
    Name = "kujhkjh",
    LastName = "dasds",
    CreationDate = DateTime.Now,
    Email = "SumThing28371OK@gmail.com",
    PhoneNumber = "+989111111111"
};

var cont = normalBank.AddAccount(
    generalAccount
);
Authentication authentication = new();

authentication.AddAuthenticationFor(
    new BasicAuth
    {
        Username = "asdasd",
        HashedPassword = "KLALdjk"
    }, cont.integral
);
authentication.PrintAll();
Console.WriteLine($"{cont.integral} {cont.account.Name}");
normalBank.AddAccount( anotherAccount );

generalAccount.Withdraw(2 * Math.Pow(2, 100), null);
anotherAccount.Withdraw(2 * Math.Pow(2, 100), null);

generalAccount.Deposit(Math.Pow(2, 1.847183), anotherAccount);


void println(IReadOnlyList<TransactionLog> accounts)
{

    foreach (var item in accounts)
    {
        Console.WriteLine(item.Action);
    }
}


println(generalAccount.GetHistory());
println(anotherAccount.GetHistory());

Console.WriteLine(generalAccount.GetHistory());
Console.WriteLine(anotherAccount.GetHistory());

Console.WriteLine(generalAccount.BankingProvider);
