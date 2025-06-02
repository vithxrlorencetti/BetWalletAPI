using BetWalletAPI.Application.DTOs;
using BetWalletAPI.Application.DTOs.Player;

namespace BetWalletAPI.Application.Interfaces.Services;

public interface IPlayerService
{
    Task<PlayerResponseDto?> GetPlayerByIdAsync(Guid id);
    Task<PlayerResponseDto?> CreatePlayerAsync(CreatePlayerRequestDto createPlayerRequestDto);
    Task<AuthResponseDto> LoginAsync(LoginPlayerRequestDto loginPlayerDto);
}
