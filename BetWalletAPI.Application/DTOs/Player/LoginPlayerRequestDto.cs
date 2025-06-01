using System.ComponentModel.DataAnnotations;

namespace BetWalletAPI.Application.DTOs.Player;

public class LoginPlayerRequestDto
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
