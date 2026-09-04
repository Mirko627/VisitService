using VisitService.Shared.dtos;

namespace VisitService.ClientHttp.Interfaces
{
    public interface IVisitClient
    {
        Task AddAsync(CreateVisitDto dto, CancellationToken ct = default);
        Task ConfirmAsync(int id, CancellationToken ct = default);
        Task RejectAsync(int id, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task UpdateAsync(int id, UpdateVisitDto dto, CancellationToken ct = default);
        Task<List<VisitDto>> GetAllAsync(CancellationToken ct = default);
        Task<VisitDto?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
