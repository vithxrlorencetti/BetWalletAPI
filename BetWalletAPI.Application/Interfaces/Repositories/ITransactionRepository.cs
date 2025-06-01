using BetWalletAPI.Application.Common;
using BetWalletAPI.Domain.Entities;

namespace BetWalletAPI.Application.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<PagedResult<Transaction>> GetByWalletIdAsync(Guid walletId, int pageNumber, int pageSize);
    Task<Transaction> AddAsync(Transaction transaction);
}