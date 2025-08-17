using EFCore.Model;

namespace EFCore.Repositories.Interfaces
{
    public interface IUnitRepository : IRepository<Unit>
    {
        Task<Unit?> GetUnitByCodeAsync(string code);
        Task<IEnumerable<Unit>> GetUnitsByTypeAsync(string unitType);
        Task<IEnumerable<Unit>> GetUnitsByNameAsync(string unitName);
    }
}