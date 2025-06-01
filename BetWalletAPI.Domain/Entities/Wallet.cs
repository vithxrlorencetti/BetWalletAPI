using BetWalletAPI.Domain.Common;
using BetWalletAPI.Domain.Enums;
using BetWalletAPI.Domain.ValueObjects;

namespace BetWalletAPI.Domain.Entities;

public class Wallet : BaseEntity
{
    public Guid PlayerId { get; private set; }
    public Player Player { get; private set; }
    public Money Balance { get; private set; }

    private readonly List<Transaction> _transactions = new();
    public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();

    private Wallet() { }

    public static Wallet Create(Guid playerId, Money initialBalance)
    {
        var wallet = new Wallet
        {
            PlayerId = playerId,
            Balance = initialBalance,
        };

        return wallet;
    }

    public bool CanAfford(Money amount)
    {
        if (amount.Currency != Balance.Currency)
            throw new InvalidOperationException("Cannot compare amounts in different currencies.");

        return amount.IsPositive && Balance.Amount >= amount;
    }

    public Transaction Credit(Money amount, TransactionType type, string description, Guid? betId = null)
    {
        if (amount.Currency != Balance.Currency)
            throw new InvalidOperationException("Transaction currency must match wallet currency.");

        if (amount.IsNegative || amount.IsZero)
            throw new ArgumentException("Credit amount must be positive.", nameof(amount));

        Balance = Balance.Add(amount);

        var transaction = Transaction.Create(this.Id, amount, type, description, betId);
        _transactions.Add(transaction);

        SetUpdatedAt();
        return transaction;
    }

    public Transaction Debit(Money amount, TransactionType type, string description, Guid? betId = null)
    {
        if (amount.Currency != Balance.Currency)
            throw new InvalidOperationException("Transaction currency must match wallet currency.");

        if (amount.IsNegative || amount.IsZero)
            throw new ArgumentException("Debit amount must be positive.", nameof(amount));

        if (!CanAfford(amount))
            throw new InvalidOperationException("Insufficient funds for debit.");

        Balance = Balance.Subtract(amount);

        var transaction = Transaction.Create(this.Id, amount, type, description, betId);
        _transactions.Add(transaction);

        SetUpdatedAt();
        return transaction;
    }

    public Transaction RecordDeposit(Money amount, string description = "Account deposit")
    {
        return Credit(amount, TransactionType.Deposit, description);
    }

    public Transaction RecordBonus(Money amount, string description = "Bonus awarded")
    {
        return Credit(amount, TransactionType.Bonus, description);
    }
}
