using BetWalletAPI.Domain.Entities;

namespace BetWalletAPI.Application.Interfaces.Repositories;

public interface IBetRepository
{
    Task<Bet?> GetByIdAsync(Guid id);
    Task<IEnumerable<Bet>> GetByPlayerIdAsync(Guid playerId);
    Task<Bet> AddAsync(Bet bet);
    Task UpdateAsync(Bet bet);
}