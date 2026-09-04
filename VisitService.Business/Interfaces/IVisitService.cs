using VisitService.Shared.dtos;

namespace VisitService.Business.Interfaces
{
    public interface IVisitService
    {
        Task<List<VisitDto>> GetAllAsync(int userId, CancellationToken ct = default);
        Task<VisitDto> GetByIdAsync(int id, int userId, CancellationToken ct = default);
        Task AddAsync(CreateVisitDto visitDto, int userId, CancellationToken ct = default);
        Task UpdateAsync(int id, UpdateVisitDto visitDto, int userId, CancellationToken ct = default);
        Task DeleteAsync(int id, int userId, CancellationToken ct = default);
        Task ConfirmVisitAsync(int visitId, int userId, CancellationToken ct = default);
        Task RejectVisitAsync(int visitId, int userId, CancellationToken ct = default);
    }
}
