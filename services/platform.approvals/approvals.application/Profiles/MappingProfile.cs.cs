using approvals.application.DTOs.ApplicationType;
using approvals.domain.Entities;
using AutoMapper;

namespace approvals.application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApprovalTemplate, GetApplicationTypeDto>().ReverseMap();
            CreateMap<ApprovalTemplate, CreateApplicationTypeDto>().ReverseMap();
            CreateMap<ApprovalTemplate, UpdateApplicationTypeDto>().ReverseMap();
        }
    }
}
