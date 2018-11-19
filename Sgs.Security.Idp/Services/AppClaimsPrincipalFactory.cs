using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Sgs.Security.Idp.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sgs.Security.Idp.Services
{
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<SgsUser, IdentityRole>
    {
        public AppClaimsPrincipalFactory(
            UserManager<SgsUser> userManager
            , RoleManager<IdentityRole> roleManager
            , IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
        { }

        public async override Task<ClaimsPrincipal> CreateAsync(SgsUser user)
        {
            var principal = await base.CreateAsync(user);

 
            ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.GivenName, user.Name)
            });

            ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                new Claim("EmployeeId", user.EmployeeId.ToString()),
            });


            return principal;
        }
    }
}
