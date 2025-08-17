using EFCore.Model;

namespace EFCore.Repository
{
    public interface IUnitRepository : IRepository<Unit>
    {
        Task<Unit> GetByCodeAsync(string code);
        Task<IEnumerable<Unit>> GetByTypeAsync(string unitType);
    }
}