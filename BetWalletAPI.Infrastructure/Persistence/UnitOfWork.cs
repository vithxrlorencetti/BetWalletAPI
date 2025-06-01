using BetWalletAPI.Application.Interfaces.Persistence;
using BetWalletAPI.Application.Interfaces.Repositories;
using BetWalletAPI.Infrastructure.Repositories;

namespace BetWalletAPI.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private bool _disposed = false;

        public IPlayerRepository PlayerRepository { get; private set; }
        public IBetRepository BetRepository { get; private set; }
        public ITransactionRepository TransactionRepository { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            PlayerRepository = new PlayerRepository(_context);
            //BetRepository = new BetRepository(_context);
            //TransactionRepository = new TransactionRepository(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
