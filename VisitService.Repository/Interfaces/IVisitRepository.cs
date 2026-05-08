using VisitService.Repository.Entities;

namespace VisitService.Repository.Interfaces
{
    public interface IVisitRepository
    {
        public Task<List<Visit>> GetAllAsync();
        public Task<Visit?> GetByIdAsync(int id);
        public Task AddAsync(Visit visit);
        public Task UpdateAsync(Visit visit);
        public Task DeleteAsync(int id);
        public Task<List<Visit>> GetByUserIdAsync(int userId);

    }
}
