using AutoMapper;
using blog_api.models.DTOs.Http;
using blog_api.models.v1;

namespace blog_api.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserReqDto>().ReverseMap();
            CreateMap<User, UserResDto>().ReverseMap();
            CreateMap<UserReqDto, UserResDto>().ReverseMap();
        }
    }
}
