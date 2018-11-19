using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sgs.Security.Idp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Security.Idp.ViewComponents
{
    public class UserInfoViewComponent : ViewComponent
    {

        private readonly UserManager<SgsUser> _userManager;

        public UserInfoViewComponent(UserManager<SgsUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return View(user);
        }

    }
}
