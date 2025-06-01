using BetWalletAPI.Domain.Common;
using BetWalletAPI.Domain.ValueObjects;

namespace BetWalletAPI.Domain.Entities;

public class Player : BaseEntity
{
    public string Username { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public Wallet Wallet { get; private set; }
    public int ConsecutiveLosses { get; private set; }
    
    private Player() { }

    public static Player Create(string username, Email email, string hashedPassword, Money initialBalance)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Player name cannot be empty.", nameof(username));

        if (initialBalance == null)
            throw new ArgumentNullException(nameof(initialBalance), "Initial balance cannot be null.");

        var player = new Player
        {
            Username = username,
            Email = email,
            PasswordHash = hashedPassword,
            ConsecutiveLosses = 0,
            CreatedAt = DateTime.UtcNow,
        };

        player.Wallet = Wallet.Create(player.Id, initialBalance);

        return player;
    }

    public void IncrementConsecutiveLosses()
    {
        ConsecutiveLosses++;
        SetUpdatedAt();
    }

    public void ResetConsecutiveLosses()
    {
        ConsecutiveLosses = 0;
        SetUpdatedAt();
    }

    public bool IsEligibleForBonus()
    {
        return ConsecutiveLosses >= 5;
    }
}
