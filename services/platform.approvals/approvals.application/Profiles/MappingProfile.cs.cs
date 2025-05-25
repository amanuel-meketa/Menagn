using approvals.application.DTOs;
using approvals.domain.Entities;
using AutoMapper;

namespace approvals.application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Approval, ApprovalDto>().ReverseMap();
        }
    }
}
