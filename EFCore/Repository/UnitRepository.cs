using EFCore.Model;
using Microsoft.EntityFrameworkCore;
using TuyenDanQuan.Data;

namespace EFCore.Repository
{
    public class UnitRepository : Repository<Unit>, IUnitRepository
    {
        public UnitRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Unit> GetByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Code == code);
        }

        public async Task<IEnumerable<Unit>> GetByTypeAsync(string unitType)
        {
            return await _dbSet.Where(u => u.UnitType == unitType).ToListAsync();
        }
    }
}