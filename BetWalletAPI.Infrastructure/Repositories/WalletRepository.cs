using BetWalletAPI.Application.Interfaces.Repositories;
using BetWalletAPI.Domain.Entities;
using BetWalletAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BetWalletAPI.Infrastructure.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly AppDbContext _context;

    public WalletRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Wallet?> GetByIdAsync(Guid id)
    {
        return await _context.Wallets.FindAsync(id);
    }

    public async Task<Wallet?> GetByPlayerIdAsync(Guid playerId)
    {
        return await _context.Wallets
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.PlayerId == playerId);
    }

    public async Task<Wallet> AddAsync(Wallet wallet)
    {
        if (wallet == null) throw new ArgumentNullException(nameof(wallet));

        await _context.Wallets.AddAsync(wallet);
       
        return wallet;
    }

    public Task UpdateAsync(Wallet wallet)
    {
        if (wallet == null) throw new ArgumentNullException(nameof(wallet));

        _context.Entry(wallet).State = EntityState.Modified;

        return Task.CompletedTask;
    }
}
