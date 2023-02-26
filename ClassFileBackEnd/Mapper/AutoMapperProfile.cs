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
        }
    }
}
