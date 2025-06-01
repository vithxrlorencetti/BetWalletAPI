using BetWalletAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BetWalletAPI.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Player> Players { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Bet> Bets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
