using EFCore.Model;

namespace EFCore.Repositories.Interfaces
{
    public interface ICitizenRepository : IRepository<Citizen>
    {
        Task<IEnumerable<Citizen>> GetCitizensByNameAsync(string name);
        Task<Citizen?> GetCitizenByIdentificationNumberAsync(int identificationNumber);
        Task<IEnumerable<Citizen>> GetCitizensByAgeRangeAsync(int minAge, int maxAge);
    }
}