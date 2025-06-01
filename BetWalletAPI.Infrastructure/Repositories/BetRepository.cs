using BetWalletAPI.Application.Common;
using BetWalletAPI.Application.Interfaces.Repositories;
using BetWalletAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetWalletAPI.Infrastructure.Persistence.Repositories;

public class BetRepository : IBetRepository
{
    private readonly AppDbContext _context;

    public BetRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Bet?> GetByIdAsync(Guid id)
    {
        return await _context.Bets.FindAsync(id);
    }

    public async Task<PagedResult<Bet>> GetByPlayerIdAsync(Guid playerId, int pageNumber, int pageSize)
    {
        if (pageNumber <= 0) throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");

        var query = _context.Bets
            .Where(b => b.PlayerId == playerId)
            .OrderByDescending(b => b.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Bet>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Bet> AddAsync(Bet bet)
    {
        if (bet == null) throw new ArgumentNullException(nameof(bet));

        await _context.Bets.AddAsync(bet);
        
        return bet;
    }

    public Task UpdateAsync(Bet bet)
    {
        if (bet == null) throw new ArgumentNullException(nameof(bet));

        _context.Entry(bet).State = EntityState.Modified;
        
        return Task.CompletedTask;
    }
}
