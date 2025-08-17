using EFCore.Model;

namespace EFCore.Repository
{
    public interface ICitizenRepository : IRepository<Citizen>
    {
        Task<IEnumerable<Citizen>> GetByFullNameAsync(string fullName);
        Task<Citizen> GetByIdentificationNumberAsync(int identificationNumber);
    }
}