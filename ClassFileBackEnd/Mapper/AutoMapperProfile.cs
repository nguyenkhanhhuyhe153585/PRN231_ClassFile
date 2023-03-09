using AutoMapper;
using ClassFileBackEnd.Models;

namespace ClassFileBackEnd.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Class, ClassDTO>();
            CreateMap<Account, AccountDTO>();
            CreateMap<Account, AccountProfileDTO>();
            CreateMap<AccountSignupDTO, Account>();
            CreateMap<Post, PostInClassDTO>();
            CreateMap<ClassFileBackEnd.Models.File, FileDTO>();
            CreateMap<Class, ClassCreateDTO>();
            CreateMap<ClassCreateDTO, Class>();
        }
    }
}
