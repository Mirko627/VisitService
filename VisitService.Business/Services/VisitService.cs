using AutoMapper;
using VisitService.Business.Interfaces;
using PropertyService.ClientHttp.Interfaces;
using PropertyService.Shared.dtos;
using VisitService.Repository.Entities;
using VisitService.Repository.Interfaces;
using VisitService.Shared.dtos;
using VisitService.Shared.enums;
using VisitService.Kafka.Producer;
using VisitService.Shared.kafka.Contracts;

namespace VisitService.Business.Services
{
    public class VisitService : IVisitService
    {
        private readonly IVisitRepository repository;
        private readonly IPropertyClient propertyClient;
        private readonly IVisitEventPublisher eventPublisher;
        private readonly IMapper mapper;
        private readonly IMapper mapperEvent;

        public VisitService(IVisitRepository repository, IPropertyClient propertyClient, IVisitEventPublisher eventPublisher, IMapper mapper, IMapper mapperEvent)
        {
            this.repository = repository;
            this.propertyClient = propertyClient;
            this.eventPublisher = eventPublisher;
            this.mapper = mapper;
            this.mapperEvent = mapperEvent;
        }

        public async Task AddAsync(CreateVisitDto visitDto, int userId, CancellationToken ct = default)
        {
            PropertyDto property = await propertyClient.GetByIdAsync(visitDto.PropertyId, ct) ?? throw new KeyNotFoundException("Immobile non esistente");

            if (property.Status != PropertyService.Shared.enums.PropertyStatus.Available)
                throw new InvalidOperationException("L'immobile non è disponibile");

            if (property.OwnerId == userId)
                throw new UnauthorizedAccessException("Non è possibile prenotare una visita per un proprio immobile");
            if(visitDto.VisitDate < DateTime.UtcNow) throw new InvalidOperationException("Non è possibile prenotare una visita per questa data");
            Visit visit = mapper.Map<Visit>(visitDto);
            visit.VisitatorId = userId;
            visit.CreatedAt = DateTime.UtcNow;
            visit.Status = VisitStatus.Pending;
            visit.OwnerId = property.OwnerId;

            VisitCreatedDto visitCreatedDto = mapperEvent.Map<VisitCreatedDto>(visit);

            OutboxEvent outboxEvent = eventPublisher.CreateVisitCreatedEvent(visitCreatedDto);

            await repository.AddAsync(visit, outboxEvent, ct);
        }

        public async Task ConfirmVisitAsync(int visitId, int userId, CancellationToken ct = default)
        {
            Visit visit = await GetVisit(visitId, ct);
            await CheckCompletedAsync(visit, ct);

            if (visit.Status != VisitStatus.Pending)
                throw new InvalidOperationException("La visita non è confermabile");

            PropertyDto property = await propertyClient.GetByIdAsync(visit.PropertyId, ct) ?? throw new KeyNotFoundException("Immobile non esistente");

            if (property.OwnerId != userId)
                throw new UnauthorizedAccessException("Solo il proprietario può confermare la visita");

            if (property.Status != PropertyService.Shared.enums.PropertyStatus.Available)
                throw new InvalidOperationException("L'immobile non è disponibile");

            visit.Status = VisitStatus.Confirmed;

            VisitConfirmedDto visitConfirmedDto = mapperEvent.Map<VisitConfirmedDto>(visit);
            
            OutboxEvent outboxEvent = eventPublisher.CreateVisitConfirmedEvent(visitConfirmedDto);

            await repository.UpdateAsync(visit, outboxEvent, ct);

        }

        public async Task RejectVisitAsync(int visitId, int userId, CancellationToken ct = default)
        {
            Visit visit = await GetVisit(visitId, ct);
            await CheckCompletedAsync(visit, ct);

            if (visit.Status != VisitStatus.Pending)
                throw new InvalidOperationException("La visita non è rifiutabile");

            PropertyDto property = await propertyClient.GetByIdAsync(visit.PropertyId, ct) ?? throw new KeyNotFoundException("Immobile non esistente");

            if (property.OwnerId != userId)
                throw new UnauthorizedAccessException("Solo il proprietario può rifiutare la visita");

            visit.Status = VisitStatus.Cancelled;

            VisitRejectedDto visitRejectedDto = mapperEvent.Map<VisitRejectedDto>(visit);

            OutboxEvent outboxEvent = eventPublisher.CreateVisitRejectedEvent(visitRejectedDto);

            await repository.UpdateAsync(visit, outboxEvent, ct);
        }

        public async Task DeleteAsync(int visitId, int userId, CancellationToken ct = default)
        {
            Visit visit = await GetVisit(visitId, ct);
            await CheckCompletedAsync(visit, ct);

            if (visit.VisitatorId != userId)
                throw new UnauthorizedAccessException("Solo il visitatore può eliminare la visita");

            if (visit.Status != VisitStatus.Pending)
                throw new InvalidOperationException("La visita non può essere eliminata");

            await repository.DeleteAsync(visitId, null, ct);
        }

        public async Task UpdateAsync(int visitId, UpdateVisitDto visitDto, int userId, CancellationToken ct = default)
        {
            Visit visit = await GetVisit(visitId, ct);
            await CheckCompletedAsync(visit, ct);

            if (visit.VisitatorId != userId)
                throw new UnauthorizedAccessException("Solo il visitatore può modificare la visita");

            if (visit.Status != VisitStatus.Pending)
                throw new InvalidOperationException("La visita non è modificabile");

            if (visitDto.VisitDate < DateTime.UtcNow) throw new InvalidOperationException("Non è possibile prenotare una visita per questa data");

            visit.VisitDate = visitDto.VisitDate;
            await repository.UpdateAsync(visit, null, ct);
        }

        public async Task<List<VisitDto>> GetAllAsync(int userId, CancellationToken ct = default)
        {
            List<Visit> visits = await repository.GetByUserIdAsync(userId, ct);

            foreach (Visit visit in visits)
                await CheckCompletedAsync(visit, ct);

            return mapper.Map<List<VisitDto>>(visits);
        }

        public async Task<VisitDto> GetByIdAsync(int visitId, int userId, CancellationToken ct = default)
        {
            Visit visit = await GetVisit(visitId, ct);
            await CheckCompletedAsync(visit, ct);

            if (visit.OwnerId != userId && visit.VisitatorId != userId)
                throw new UnauthorizedAccessException("Accesso negato alla visita");

            return mapper.Map<VisitDto>(visit);
        }

        private async Task<Visit> GetVisit(int visitId, CancellationToken ct = default)
        {
            return await repository.GetByIdAsync(visitId, ct) ?? throw new KeyNotFoundException("La visita non esiste");
        }

        private async Task CheckCompletedAsync(Visit visit, CancellationToken ct = default)
        {
            if (visit.Status != VisitStatus.Completed && visit.VisitDate < DateTime.UtcNow)
            {
                visit.Status = VisitStatus.Completed;

                VisitCompletedDto visitCompletedDto = mapperEvent.Map<VisitCompletedDto>(visit);

                OutboxEvent outboxEvent = eventPublisher.CreateVisitCompletedEvent(visitCompletedDto);

                await repository.UpdateAsync(visit, outboxEvent, ct);
            }
        }
    }
}

