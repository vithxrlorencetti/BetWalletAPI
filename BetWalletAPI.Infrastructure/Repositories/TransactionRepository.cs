using BetWalletAPI.Application.Common;
using BetWalletAPI.Application.Interfaces.Repositories;
using BetWalletAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetWalletAPI.Infrastructure.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await _context.Transactions.FindAsync(id);
    }

    public async Task<PagedResult<Transaction>> GetByWalletIdAsync(Guid walletId, int pageNumber, int pageSize)
    {
        if (pageNumber <= 0) throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");

        var query = _context.Transactions
            .Where(t => t.WalletId == walletId)
            .OrderByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Transaction>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Transaction> AddAsync(Transaction transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));

        await _context.Transactions.AddAsync(transaction);
        
        return transaction;
    }
}
