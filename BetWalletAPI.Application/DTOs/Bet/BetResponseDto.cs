using System;
using BetWalletAPI.Domain.Enums;

namespace BetWalletAPI.Application.DTOs.Bet;

public class BetResponseDto
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public decimal Stake { get; set; }
    public string Currency { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public decimal? Prize { get; set; }
    public Guid? BetTransactionId { get; set; }
    public Guid? PrizeTransactionId { get; set; }
    public Guid? RefundTransactionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
