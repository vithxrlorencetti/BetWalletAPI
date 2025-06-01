using BCryptNet = BCrypt.Net.BCrypt;
using BetWalletAPI.Application.Interfaces.Security;

namespace BetWalletAPI.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");
        }
        return BCryptNet.HashPassword(password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(providedPassword))
        {
            throw new ArgumentNullException(string.IsNullOrEmpty(hashedPassword) ? nameof(hashedPassword) : nameof(providedPassword));
        }

        try
        {
            return BCryptNet.Verify(providedPassword, hashedPassword);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}