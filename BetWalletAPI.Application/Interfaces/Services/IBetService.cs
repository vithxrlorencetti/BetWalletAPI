using BetWalletAPI.Application.Common;
using BetWalletAPI.Application.DTOs.Bet;
using BetWalletAPI.Domain.Entities;
using BetWalletAPI.Domain.ValueObjects;

namespace BetWalletAPI.Application.Interfaces.Services;

public interface IBetService
{
    Task<BetResponseDto?> PlaceBetAsync(PlaceBetRequestDto placeBetRequestDto);
    Task<BetResponseDto?> SettleBetAsWonAsync(Guid betId);
    Task<BetResponseDto?> SettleBetAsLostAsync(Guid betId);
    Task<BetResponseDto?> CancelBetAsync(Guid betId);
    Task<BetResponseDto?> GetBetByIdAsync(Guid betId);
    Task<PagedResult<BetResponseDto>> GetBetsByPlayerAsync(Guid playerId, int pageNumber, int pageSize);
}
