namespace BetWalletAPI.Application.Interfaces.Security;

public interface IPasswordHasher
{
    string Hash(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}