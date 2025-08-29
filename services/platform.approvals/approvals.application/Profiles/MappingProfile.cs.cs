using approvals.application.DTOs.ApplicationType;
using approvals.application.DTOs.ApprovalInstance;
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

            CreateMap<ApprovalInstance, GetApprovalInstanceDto>().ReverseMap();
            CreateMap<ApprovalInstance, CreateApprovaleInstanceDto>().ReverseMap();
            CreateMap<ApprovalInstance, UpdateApprovaleInstanceDto>().ReverseMap();
            CreateMap<GetApprovalInstanceDto, UpdateApprovaleInstanceDto>().ReverseMap();
            CreateMap<ApprovalInstance, GetMyApprovalInstanceDto>()
              .ForMember(dest => dest.TemplateName, opt => opt.MapFrom(src => src.Template != null ? src.Template.Name : null));

            CreateMap<StageDefinition, GetStageDefinitionDto>().ReverseMap();
            CreateMap<StageDefinition, CreateStageDefinitionDto>().ReverseMap();
            CreateMap<StageDefinition, UpdateStageDefinitionDto>().ReverseMap();

            CreateMap<StageInstance, GetStageInstanceDto>().ReverseMap();
            CreateMap<StageInstance, CreateStageInstanceDto>().ReverseMap();
            CreateMap<StageInstance, UpdateApprovaleInstanceDto>().ReverseMap();
        }
    }
}
