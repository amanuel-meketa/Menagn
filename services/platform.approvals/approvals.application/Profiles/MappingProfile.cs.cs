using approvals.application.DTOs.ApplicationType;
using approvals.domain.Entities;
using AutoMapper;

namespace approvals.application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationType, ApplicationTypeDto>().ReverseMap();
            CreateMap<ApplicationType, CreateApplicationTypeDto>().ReverseMap();
        }
    }
}
