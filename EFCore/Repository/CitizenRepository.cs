using EFCore.Model;
using Microsoft.EntityFrameworkCore;
using TuyenDanQuan.Data;

namespace EFCore.Repository
{
    public class CitizenRepository : Repository<Citizen>, ICitizenRepository
    {
        public CitizenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Citizen>> GetByFullNameAsync(string fullName)
        {
            return await _dbSet.Where(c => c.FullName.Contains(fullName)).ToListAsync();
        }

        public async Task<Citizen> GetByIdentificationNumberAsync(int identificationNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.IdentificationNumber == identificationNumber);
        }
    }
}