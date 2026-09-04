using VisitService.Repository.Entities;

namespace VisitService.Repository.Interfaces
{
    public interface IVisitRepository
    {
        public Task<List<Visit>> GetAllAsync(CancellationToken ct = default);
        public Task<Visit?> GetByIdAsync(int id, CancellationToken ct = default);
        public Task AddAsync(Visit visit, OutboxEvent? outboxEvent = null, CancellationToken ct = default);
        public Task UpdateAsync(Visit visit, OutboxEvent? outboxEvent = null, CancellationToken ct = default);
        public Task DeleteAsync(int id, OutboxEvent? outboxEvent = null, CancellationToken ct = default);
        public Task<List<Visit>> GetByUserIdAsync(int userId, CancellationToken ct = default);

    }
}
