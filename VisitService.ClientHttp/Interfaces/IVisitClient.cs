using VisitService.Shared.dtos;

namespace VisitService.ClientHttp.Interfaces
{
    public interface IVisitClient
    {
        Task AddAsync(CreateVisitDto dto);
        Task ConfirmAsync(int id);
        Task RejectAsync(int id);
        Task DeleteAsync(int id);
        Task UpdateAsync(int id, UpdateVisitDto dto);
        Task<List<VisitDto>> GetAllAsync();
        Task<VisitDto?> GetByIdAsync(int id);
    }
}
