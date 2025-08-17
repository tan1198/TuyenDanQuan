using EFCore.Model;
using EFCore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using TuyenDanQuan.Data;

namespace EFCore.Repositories.Implementations
{
    public class UnitRepository : Repository<Unit>, IUnitRepository
    {
        public UnitRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Unit?> GetUnitByCodeAsync(string code)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Code == code);
        }

        public async Task<IEnumerable<Unit>> GetUnitsByTypeAsync(string unitType)
        {
            return await _dbSet
                .Where(u => u.UnitType == unitType)
                .ToListAsync();
        }

        public async Task<IEnumerable<Unit>> GetUnitsByNameAsync(string unitName)
        {
            return await _dbSet
                .Where(u => u.UnitName.Contains(unitName))
                .ToListAsync();
        }
    }
}