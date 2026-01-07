using VisitService.Shared.dtos;

namespace VisitService.Business.Interfaces
{
    public interface IVisitService
    {
        public Task<List<VisitDto>> GetAllAsync(int userId);
        public Task<VisitDto> GetByIdAsync(int id, int userId);
        public Task AddAsync(CreateVisitDto visitDto, int userId);
        public Task UpdateAsync(int id, UpdateVisitDto visitDto, int userId);
        public Task DeleteAsync(int id, int userId);
        public Task ConfirmVisitAsync(int visitId, int userId);
        public Task RejectVisitAsync(int visitId, int userId);
    }
}
