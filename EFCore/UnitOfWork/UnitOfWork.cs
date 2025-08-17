using EFCore.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using TuyenDanQuan.Data;

namespace EFCore.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;
        
        private ICitizenRepository _citizens;
        private IUnitRepository _units;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public ICitizenRepository Citizens => 
            _citizens ??= new CitizenRepository(_context);

        public IUnitRepository Units => 
            _units ??= new UnitRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}