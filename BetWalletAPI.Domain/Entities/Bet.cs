using BetWalletAPI.Domain.Common;
using BetWalletAPI.Domain.Enums;
using BetWalletAPI.Domain.ValueObjects;

namespace BetWalletAPI.Domain.Entities;

public class Bet : BaseEntity
{
    public Guid PlayerId { get; private set; }
    public Money Stake { get; private set; }
    public BetStatus Status { get; private set; }
    public string Description { get; private set; }

    public Guid BetTransactionId { get; private set; }      // Transaction de débito
    public Guid? PrizeTransactionId { get; private set; }   // Transaction de crédito (se ganhar)
    public Guid? RefundTransactionId { get; private set; }  // Transaction de estorno (se cancelar)

    private Bet() { }

    public static Bet Create(Guid playerId, Money stake, string description = "Bet created")
    {
        if (stake.IsNegative || stake.IsZero || stake <= 1m)
            throw new ArgumentException($"Bet stake must be at least {new Money(1.00m, stake.Currency)}.", nameof(stake));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Bet description cannot be empty.", nameof(description));

        return new Bet
        {
            PlayerId = playerId,
            Stake = stake,
            Status = BetStatus.Pending,
            Description = description
        };
    }

    public void SettleAsWon(Guid prizeTransactionId)
    {
        if (Status != BetStatus.Pending)
            throw new InvalidOperationException($"Cannot settle bet as won. Current status: {Status}");

        Status = BetStatus.Won;
        PrizeTransactionId = prizeTransactionId;
        SetUpdatedAt();
    }

    public void SettleAsLost()
    {
        if (Status != BetStatus.Pending)
            throw new InvalidOperationException($"Cannot settle bet as lost. Current status: {Status}");

        Status = BetStatus.Lost;
        SetUpdatedAt();
    }

    public void Cancel(Guid refundTransactionId)
    {
        if (Status != BetStatus.Pending)
            throw new InvalidOperationException($"Cannot cancel bet. Current status: {Status}");

        Status = BetStatus.Cancelled;
        RefundTransactionId = refundTransactionId;
        SetUpdatedAt();
    }

    public bool CanBeCancelled => Status == BetStatus.Pending;
    public bool IsSettled => Status != BetStatus.Pending;
}