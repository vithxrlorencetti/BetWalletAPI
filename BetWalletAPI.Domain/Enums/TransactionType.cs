namespace BetWalletAPI.Domain.Enums;

public enum TransactionType
{
    BetPlacement = 1,    // Débito: jogador faz aposta
    BetWinnings = 2,     // Crédito: jogador ganha prêmio da aposta
    BetRefund = 3,       // Crédito: reembolso de aposta cancelada
    Bonus = 4,           // Crédito: bônus (ex: por perdas consecutivas)
    Deposit = 5,         // Crédito: depósito na conta
}