using VisitService.Shared.dtos;

namespace VisitService.Business.Interfaces
{
    public interface IVisitService
    {
        Task<List<VisitDto>> GetAllAsync(int userId);
        Task<VisitDto> GetByIdAsync(int id, int userId);
        Task AddAsync(CreateVisitDto visitDto, int userId);
        Task UpdateAsync(int id, UpdateVisitDto visitDto, int userId);
        Task DeleteAsync(int id, int userId);
        Task ConfirmVisitAsync(int visitId, int userId);
        Task RejectVisitAsync(int visitId, int userId);
    }
}
