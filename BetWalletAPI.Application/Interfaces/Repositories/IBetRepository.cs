using BetWalletAPI.Application.Common;
using BetWalletAPI.Domain.Entities;

namespace BetWalletAPI.Application.Interfaces.Repositories;

public interface IBetRepository
{
    Task<Bet?> GetByIdAsync(Guid id);
    Task<PagedResult<Bet>> GetByPlayerIdAsync(Guid playerId, int pageNumber, int pageSize);
    Task<Bet> AddAsync(Bet bet);
    Task UpdateAsync(Bet bet);
}