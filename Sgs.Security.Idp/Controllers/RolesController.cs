using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sgs.Security.Idp.ViewModels.Roles;

namespace Sgs.Security.Idp.Controllers
{
    public class RolesController : BaseController
    {
        private readonly RoleManager<IdentityRole> _rolesManager;

        public RolesController(RoleManager<IdentityRole> rolesManager
            ,IMapper mapper
            , ILogger<RolesController> logger) : base(mapper, logger)
        {
            this._rolesManager = rolesManager;
        }

        public async Task<IActionResult> Index()
        {
            var rolesList = await this._rolesManager.Roles.AsNoTracking().ToListAsync();

            var vm = _mapper.Map<IEnumerable<IdentityRoleViewModel>>(rolesList);

            return View(vm);
        }

        public async Task<IActionResult> RolesList()
        {
            var rolesList = await this._rolesManager.Roles.AsNoTracking().ToListAsync();

            var vm = _mapper.Map<IEnumerable<IdentityRoleViewModel>>(rolesList);

            return PartialView(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            try
            {
                var result = await _rolesManager.CreateAsync(new IdentityRole(roleName));
                if(result.Succeeded)
                {
                    return Json(new { result = true });
                }
            }
            catch (Exception)
            {
            }

            return Json(new { result = false });
        }

        public async Task<IActionResult> verifyRoleName(string roleName,string id="")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    return Json($"Sorry role name is not valid !!");
                }
                var role = await _rolesManager.FindByNameAsync(roleName);
                if (role != null && role.Id.Trim().ToLower() != id?.Trim().ToLower())
                {
                    return Json($"Sorry {roleName} is already registred !!");
                }
                return Json(true);
            }
            catch (Exception)
            {
                return Json("validation error ...!!");
            }
        }
    }
}
