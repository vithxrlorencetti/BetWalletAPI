using System.ComponentModel.DataAnnotations;

namespace BetWalletAPI.Application.DTOs.Transaction;

public class CreateDepositTransactionRequestDto
{
    [Required]
    public Guid PlayerId { get; set; }

    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = "Player Deposit";
}
