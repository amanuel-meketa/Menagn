using AutoMapper;
using security.business.Dtos.Incoming;
using security.data.Entities.Users;

namespace security.SharedUtils.Mappings
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<GetUserDto, User>().ReverseMap();
        }
    }
}
