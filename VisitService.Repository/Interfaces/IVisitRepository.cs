using VisitService.Repository.Entities;

namespace VisitService.Repository.Interfaces
{
    public interface IVisitRepository
    {
        public Task<List<Visit>> GetAllAsync();
        public Task<Visit?> GetByIdAsync(int id);
        public Task AddAsync(Visit visit, OutboxEvent? outboxEvent = null);
        public Task UpdateAsync(Visit visit, OutboxEvent? outboxEvent = null);
        public Task DeleteAsync(int id, OutboxEvent? outboxEvent = null);
        public Task<List<Visit>> GetByUserIdAsync(int userId);

    }
}
