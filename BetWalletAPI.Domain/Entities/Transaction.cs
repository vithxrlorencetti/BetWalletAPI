﻿using BetWalletAPI.Domain.Common;
using BetWalletAPI.Domain.Enums;
using BetWalletAPI.Domain.ValueObjects;

namespace BetWalletAPI.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid WalletId { get; private set; }
    public Money Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public string Description { get; private set; }
    public Guid? ReferenceBetId { get; private set; }

    private Transaction() { }

    public static Transaction Create(Guid walletId, Money amount, TransactionType type, string description, Guid? referenceBetId = null)
    {
        if (amount.IsNegative || amount.IsZero)
            throw new ArgumentException("Transaction amount must be positive.", nameof(amount));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Transaction description cannot be empty.", nameof(description));

        return new Transaction
        {
            WalletId = walletId,
            Amount = amount,
            Type = type,
            Description = description,
            ReferenceBetId = referenceBetId
        };
    }

    public void SetReferenceBet(Guid betId)
    {
        if (ReferenceBetId != null)
            throw new InvalidOperationException("Reference Bet ID has already been set.");

        if (betId == Guid.Empty)
            throw new ArgumentException("Invalid reference bet ID.", nameof(betId));

        ReferenceBetId = betId;
    }
}