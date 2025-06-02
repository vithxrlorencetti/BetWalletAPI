using BetWalletAPI.Domain.Entities;

namespace BetWalletAPI.Application.Interfaces.Repositories;

public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(Guid id);
    Task<Wallet?> GetByPlayerIdAsync(Guid playerId);
    Task<Wallet> AddAsync(Wallet wallet);
    Task UpdateAsync(Wallet wallet);
}
