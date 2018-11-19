using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sgs.Security.Idp.Models;
using Sgs.Security.Idp.Services;
using Sgs.Security.Idp.ViewModels;
using Sgs.Security.Idp.ViewModels.Account;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sgs.Security.Idp.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : BaseController
    {
        private readonly UserManager<SgsUser> _userManager;
        private readonly SignInManager<SgsUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public AccountController(
             UserManager<SgsUser> userManager,
             SignInManager<SgsUser> signInManager,
             RoleManager<IdentityRole> roleManager,
             IEmailSender emailSender,
             IMapper mapper,
             ILogger<AccountController> logger) : base(mapper,logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            ViewData["StatusMessage"] = this.StatusMessage;

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountLoginViewModel model, string button, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"{model.UserName} logged in.");
                    this.StatusMessage = "Login succeded !!";
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt - محاولة دخول خاطئة");
                    //return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            this.StatusMessage = "Canceled - تم الغاء التسجيل";
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);

                    if(user == null)
                    {
                        user = _userManager.Users.FirstOrDefault(u => u.EmployeeId == model.EmployeeId);
                        if(user != null)
                        {
                            ModelState.AddModelError(nameof(UserRegisterViewModel.EmployeeId), "Employee id already registered !");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(UserRegisterViewModel.UserName), "Username already taken !");
                    }

                    if(user==null)
                    {
                        bool isSuperUser = !_userManager.Users.Any();

                        user = _mapper.Map<SgsUser>(model);

                        var result = await _userManager.CreateAsync(user, model.Password);

                        if (result.Succeeded)
                        {
                            _logger.LogInformation("User created a new account with password.");

                            if(isSuperUser)
                            {
                                if (!(await _roleManager.RoleExistsAsync("Admin")))
                                {
                                    var adminRole = new IdentityRole("Admin");
                                    await _roleManager.CreateAsync(adminRole);
                                    await _roleManager.AddClaimAsync(adminRole, new Claim("IsAdmin", "True"));
                                }

                                var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
                                var claimResult = await _userManager.AddClaimAsync(user, new Claim("SuperUser", "True"));

                                if (!roleResult.Succeeded && !claimResult.Succeeded)
                                {
                                    AddErrors(roleResult);
                                    return View(model);
                                }
                            }

                            await _signInManager.SignInAsync(user, isPersistent: false);
                            _logger.LogInformation("User created a new account with password.");
                            this.StatusMessage = "User Register Success !!";
                            return RedirectToLocal(returnUrl);
                        }
                        AddErrors(result);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception thrown while register: {ex}");
                    throw ex;
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            this.StatusMessage = "Logout succeded !!";
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
