using AutoMapper;
using VisitService.Repository.Entities;
using VisitService.Shared.dtos;

namespace VisitService.Business.Mappers
{
    public class VisitMapper : Profile
    {
        public VisitMapper() {
            CreateMap<Visit, VisitDto>();

            CreateMap<UpdateVisitDto, Visit>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<CreateVisitDto, Visit>();
        }
    }
}
