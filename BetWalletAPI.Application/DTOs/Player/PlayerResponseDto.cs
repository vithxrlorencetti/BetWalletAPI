using System.ComponentModel.DataAnnotations;

namespace BetWalletAPI.Application.DTOs.Player;

public class PlayerResponseDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; }
}
