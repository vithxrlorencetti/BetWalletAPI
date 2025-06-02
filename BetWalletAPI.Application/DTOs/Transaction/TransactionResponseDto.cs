using BetWalletAPI.Domain.Enums;

namespace BetWalletAPI.Application.DTOs.Transaction;

public class TransactionResponseDto
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public Guid? ReferenceBetId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
