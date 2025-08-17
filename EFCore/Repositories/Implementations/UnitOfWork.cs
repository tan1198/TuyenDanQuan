using EFCore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using TuyenDanQuan.Data;

namespace EFCore.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        private ICitizenRepository? _citizenRepository;
        private IUnitRepository? _unitRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public ICitizenRepository Citizens
        {
            get
            {
                return _citizenRepository ??= new CitizenRepository(_context);
            }
        }

        public IUnitRepository Units
        {
            get
            {
                return _unitRepository ??= new UnitRepository(_context);
            }
        }

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
            try
            {
                await SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
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
            _context.Dispose();
        }
    }
}