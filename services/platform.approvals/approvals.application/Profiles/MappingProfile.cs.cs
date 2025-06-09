using approvals.application.DTOs.ApplicationType;
using approvals.application.DTOs.StageDefinition;
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

            CreateMap<StageDefinition, GetStageDefinitionDto>().ReverseMap();
            CreateMap<StageDefinition, CreateStageDefinitionDto>().ReverseMap();
            CreateMap<StageDefinition, UpdateStageDefinitionDto>().ReverseMap();
        }
    }
}
