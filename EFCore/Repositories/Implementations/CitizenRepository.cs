using EFCore.Model;
using EFCore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using TuyenDanQuan.Data;

namespace EFCore.Repositories.Implementations
{
    public class CitizenRepository : Repository<Citizen>, ICitizenRepository
    {
        public CitizenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Citizen>> GetCitizensByNameAsync(string name)
        {
            return await _dbSet
                .Where(c => c.FullName.Contains(name))
                .ToListAsync();
        }

        public async Task<Citizen?> GetCitizenByIdentificationNumberAsync(int identificationNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.IdentificationNumber == identificationNumber);
        }

        public async Task<IEnumerable<Citizen>> GetCitizensByAgeRangeAsync(int minAge, int maxAge)
        {
            var today = DateTime.Today;
            var maxBirthDate = today.AddYears(-minAge);
            var minBirthDate = today.AddYears(-maxAge - 1);

            return await _dbSet
                .Where(c => c.DateOfBirth.HasValue && 
                           c.DateOfBirth >= minBirthDate && 
                           c.DateOfBirth <= maxBirthDate)
                .ToListAsync();
        }
    }
}