using BetWalletAPI.Domain.Entities;

namespace BetWalletAPI.Application.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetByPlayerIdAsync(Guid playerId);
    Task<Transaction> AddAsync(Transaction transaction);
}