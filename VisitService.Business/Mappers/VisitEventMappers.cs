using AutoMapper;
using VisitService.Repository.Entities;
using VisitService.Kafka.Contracts;

namespace VisitService.Business.Mappers
{
    public class VisitEventMapper : Profile
    {
        public VisitEventMapper()
        {

            CreateMap<Visit, VisitCompletedDto>();
            CreateMap<Visit, VisitConfirmedDto>();
            CreateMap<Visit, VisitCreatedDto>();
            CreateMap<Visit, VisitRejectedDto>();
        }
    }
}