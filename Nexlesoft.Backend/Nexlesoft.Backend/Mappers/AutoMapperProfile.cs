using AutoMapper;
using Nexlesoft.Backend.Dtos;
using Nexlesoft.Domain.Entities;

namespace Nexlesoft.Backend.Mappers
{

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Hash, opt => opt.Ignore()); 
            
        }
    }
}
