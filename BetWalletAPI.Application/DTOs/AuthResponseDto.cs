using BetWalletAPI.Application.DTOs.Player;

namespace BetWalletAPI.Application.DTOs;

public class AuthResponseDto
{
    public PlayerResponseDto Player { get; set; }
    public string Token { get; set; }
    public DateTime TokenExpiration { get; set; }

    public AuthResponseDto(PlayerResponseDto player, string token, DateTime tokenExpiration)
    {
        Player = player;
        Token = token;
        TokenExpiration = tokenExpiration;
    }
}



