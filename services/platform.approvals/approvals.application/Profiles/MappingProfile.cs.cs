using approvals.application.DTOs.ApplicationType;
using approvals.domain.Entities;
using AutoMapper;

namespace approvals.application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApprovalTemplate, GetAppTemplateDto>().ReverseMap();
            CreateMap<ApprovalTemplate, CreateApprovalTemplateDto>().ReverseMap();
            CreateMap<ApprovalTemplate, UpdatAppemplateDto>().ReverseMap();
        }
    }
}
