using EFCore.Repository;

namespace EFCore.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ICitizenRepository Citizens { get; }
        IUnitRepository Units { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}