using AutoMapper;
using VisitService.Business.Interfaces;
using PropertyService.ClientHttp.Interfaces;
using PropertyService.Shared.dtos;
using VisitService.Repository.Entities;
using VisitService.Repository.Interfaces;
using VisitService.Shared.dtos;
using VisitService.Shared.enums;

namespace VisitService.Business.Services
{
    public class VisitService : IVisitService
    {
        private readonly IVisitRepository repository;
        private readonly IPropertyClient propertyClient;
        private readonly IMapper mapper;

        public VisitService( IVisitRepository repository, IMapper mapper, IPropertyClient propertyClient)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.propertyClient = propertyClient;
        }

        public async Task AddAsync(CreateVisitDto visitDto, int userId)
        {
            PropertyDto property = await propertyClient.GetByIdAsync(visitDto.PropertyId) ?? throw new KeyNotFoundException("Immobile non esistente");

            if (property.Status == PropertyService.Shared.enums.PropertyStatus.Sold)
                throw new InvalidOperationException("L'immobile è già stato venduto");

            if (property.OwnerId == userId)
                throw new UnauthorizedAccessException("Non è possibile prenotare una visita per un proprio immobile");

            Visit visit = mapper.Map<Visit>(visitDto);
            visit.VisitatorId = userId;
            visit.CreatedAt = DateTime.UtcNow;
            visit.Status = VisitStatus.Pending;
            visit.OwnerId = property.OwnerId;

            await repository.AddAsync(visit);
        }

        public async Task ConfirmVisitAsync(int visitId, int userId)
        {
            Visit visit = await GetVisit(visitId);
            await CheckCompletedAsync(visit);

            if (visit.Status != VisitStatus.Pending)
                throw new InvalidOperationException("La visita non è confermabile");

            PropertyDto property = await propertyClient.GetByIdAsync(visit.PropertyId) ?? throw new KeyNotFoundException("Immobile non esistente");

            if (property.OwnerId != userId)
                throw new UnauthorizedAccessException("Solo il proprietario può confermare la visita");

            if (property.Status != PropertyService.Shared.enums.PropertyStatus.Available)
                throw new InvalidOperationException("L'immobile non è disponibile");

            visit.Status = VisitStatus.Confirmed;

            await repository.UpdateAsync(visit);
        }

        public async Task RejectVisitAsync(int visitId, int userId)
        {
            Visit visit = await GetVisit(visitId);
            await CheckCompletedAsync(visit);

            if (visit.Status != VisitStatus.Pending)
                throw new InvalidOperationException("La visita non è rifiutabile");

            PropertyDto property = await propertyClient.GetByIdAsync(visit.PropertyId) ?? throw new KeyNotFoundException("Immobile non esistente");

            if (property.OwnerId != userId)
                throw new UnauthorizedAccessException("Solo il proprietario può rifiutare la visita");

            visit.Status = VisitStatus.Cancelled;
            await repository.UpdateAsync(visit);
        }

        public async Task DeleteAsync(int visitId, int userId)
        {
            Visit visit = await GetVisit(visitId);
            await CheckCompletedAsync(visit);

            if (visit.VisitatorId != userId)
                throw new UnauthorizedAccessException("Solo il visitatore può eliminare la visita");

            if (visit.Status != VisitStatus.Pending)
                throw new InvalidOperationException("La visita non può essere eliminata");

            await repository.DeleteAsync(visitId);
        }

        public async Task UpdateAsync(int visitId, UpdateVisitDto visitDto, int userId)
        {
            Visit visit = await GetVisit(visitId);
            await CheckCompletedAsync(visit);

            if (visit.VisitatorId != userId)
                throw new UnauthorizedAccessException("Solo il visitatore può modificare la visita");

            if (visit.Status != VisitStatus.Pending)
                throw new InvalidOperationException("La visita non è modificabile");

            visit.VisitDate = visitDto.VisitDate;
            await repository.UpdateAsync(visit);
        }

        public async Task<List<VisitDto>> GetAllAsync(int userId)
        {
            List<Visit> visits = await repository.GetByUserIdAsync(userId);

            foreach (Visit visit in visits)
                await CheckCompletedAsync(visit);

            return mapper.Map<List<VisitDto>>(visits);
        }

        public async Task<VisitDto> GetByIdAsync(int visitId, int userId)
        {
            Visit visit = await GetVisit(visitId) ?? throw new KeyNotFoundException("Visita non esistente");
            await CheckCompletedAsync(visit);

            if (visit.OwnerId != userId && visit.VisitatorId != userId)
                throw new UnauthorizedAccessException("Accesso negato alla visita");

            return mapper.Map<VisitDto>(visit);
        }

        private async Task<Visit> GetVisit(int visitId)
        {
            return await repository.GetByIdAsync(visitId) ?? throw new KeyNotFoundException("La visita non esiste");
        }

        private async Task CheckCompletedAsync(Visit visit)
        {
            if (visit.Status != VisitStatus.Completed && visit.VisitDate < DateTime.UtcNow)
            {
                visit.Status = VisitStatus.Completed;
                await repository.UpdateAsync(visit);
            }
        }
    }
}
