using BetWalletAPI.Application.Interfaces.Repositories;

namespace BetWalletAPI.Application.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    IPlayerRepository PlayerRepository { get; }
    ITransactionRepository TransactionRepository { get; }
    IBetRepository BetRepository { get; }
    IWalletRepository WalletRepository { get; }

    Task<int> SaveChangesAsync();
}
