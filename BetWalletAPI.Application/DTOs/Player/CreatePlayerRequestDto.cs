using System.ComponentModel.DataAnnotations;

namespace BetWalletAPI.Application.DTOs.Player;

public class CreatePlayerRequestDto
{
    [Required]
    [StringLength(50, MinimumLength = 10)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }

    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Initial balance must be positive.")]
    public decimal InitialBalance { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be 3 characters long.")]
    public string Currency { get; set; }
}
