using BetWalletAPI.Application.Interfaces.Repositories;
using BetWalletAPI.Domain.Entities;
using BetWalletAPI.Domain.ValueObjects;
using BetWalletAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BetWalletAPI.Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly AppDbContext _context;

    public PlayerRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Player?> GetByIdAsync(Guid id)
    {
        return await _context.Players.Include(p => p.Wallet).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Player?> GetByEmailAsync(Email email)
    {
        return await _context.Players.Where(p => p.Email == email.Value).FirstOrDefaultAsync();

        var a = await _context.Players.ToListAsync();

        return a.FirstOrDefault();
    }

    public async Task<Player> AddAsync(Player player)
    {
        if (player == null)
        {
            throw new ArgumentNullException(nameof(player));
        }

        await _context.Players.AddAsync(player);

        return player;
    }
}
