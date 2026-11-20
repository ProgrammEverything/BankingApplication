namespace BankingApplicatione.Banking.Cryptography;
using static BankingApplicatione.Banking.StatusFactory;

using BCrypt.Net;

class Crypto
{
    private string username;
    private string password;

    public Crypto(string username, string password)
    {
        this.username = username;
        this.password = password;
    }

    public BasicAuth PublishBasic()
    {
        return new BasicAuth
        {
            Username = username,
            HashedPassword = BCrypt.HashPassword(password, 5)
        };
    }
}

class Authentication
{
    List<AuthenticationToken> Auths = new();

    public void PrintAll()
    {
        foreach (var token in Auths)
        {
            Console.WriteLine(token.UserId);
            Console.WriteLine(token.Auth.HashedPassword);
            Console.WriteLine(token.Auth.Username);
        }
    }
    public short AddAuthenticationFor(BasicAuth auth, uint id)
    {
        Auths.Add(new AuthenticationToken
        {
            UserId = id,
            Auth = new BasicAuth
            {
                Username = auth.Username,
                HashedPassword = BCrypt.HashPassword(auth.HashedPassword, 5)
            }
        });
        return StatusFactory.Success();
    }
}

class AuthenticationToken
{
    public BasicAuth Auth { get; set;  }
    public uint UserId { get; init; }
}