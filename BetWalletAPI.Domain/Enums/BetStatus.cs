namespace BetWalletAPI.Domain.Enums;

public enum BetStatus
{
    Pending = 1,     // Aposta pendente
    Won = 2,         // Aposta ganhadora
    Lost = 3,        // Aposta perdedora
    Cancelled = 4    // Aposta cancelada
}
