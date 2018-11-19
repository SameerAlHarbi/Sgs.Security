using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Sgs.Security.Idp.Models;
using Sgs.Security.Idp.ViewModels.Roles;

namespace Sgs.Security.Idp.ViewModels
{
    public class SecurityMappingProfile : Profile
    {
        public SecurityMappingProfile()
        {
            CreateMap<SgsUser, UserViewModel>().ReverseMap();
            CreateMap<SgsUser, UserRegisterViewModel>().ReverseMap();
            CreateMap<IdentityRole, IdentityRoleViewModel>().ReverseMap();
        }
    }
}
