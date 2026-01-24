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
            // ----- ApprovalTemplate -----
            CreateMap<ApprovalTemplate, GetAppTemplateDto>().ReverseMap();
            CreateMap<ApprovalTemplate, AppTemplateDto>();
            CreateMap<ApprovalTemplate, CreateApprovalTemplateDto>().ReverseMap();
            CreateMap<ApprovalTemplate, UpdatAppemplateDto>().ReverseMap();

            // ----- ApprovalInstance -----
            CreateMap<ApprovalInstance, GetAppInstanceWithStageDto>().ReverseMap();
            CreateMap<ApprovalInstance, CreateApprovaleInstanceDto>().ReverseMap();
            CreateMap<ApprovalInstance, UpdateApprovaleInstanceDto>().ReverseMap();
            CreateMap<GetAppInstanceWithStageDto, UpdateApprovaleInstanceDto>().ReverseMap();
            CreateMap<ApprovalInstance, GetAppInstanceDto>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.AllStages, opt => opt.MapFrom(src => src.StageInstances.Count));
           
            CreateMap<ApprovalInstance, GetMyApprovalInstanceDto>()
                .ForMember(dest => dest.AllStages, opt => opt.MapFrom(src => src.StageInstances.Count));


            // ----- StageDefinition -----
            CreateMap<StageDefinition, GetStageDefinitionDto>().ReverseMap();
            CreateMap<StageDefinition, CreateStageDefinitionDto>().ReverseMap();
            CreateMap<StageDefinition, UpdateStageDefinitionDto>().ReverseMap();

            // ----- StageInstance -----
            CreateMap<StageInstance, GetStageInstanceDto>().ReverseMap();
            CreateMap<StageInstance, CreateStageInstanceDto>().ReverseMap();

            CreateMap<UserInfo, UserInfoDto>().ReverseMap();
        }
    }
}
