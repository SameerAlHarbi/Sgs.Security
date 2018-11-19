using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sgs.Security.Idp.Models;
using Sgs.Security.Idp.ViewModels;

namespace Sgs.Security.Idp.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly UserManager<SgsUser> _userManager;

        public UsersController(UserManager<SgsUser> usersMgr,
            IMapper mapper,
            ILogger<UsersController> logger) : base(mapper,logger)
        {
            this._userManager = usersMgr;
        }

        public IActionResult Index()
        {
            ViewData["StatusMessage"] = StatusMessage;
            //TempData.Keep();
            var usersList = this._userManager.Users.ToList().Select(u => _mapper.Map<UserViewModel>(u)).ToList();

            return View(usersList);
        }

        public IActionResult Details(int employeeId)
        {
            var userDetail = this._userManager
                .Users.FirstOrDefault(u => u.EmployeeId == employeeId);

            if (userDetail != null)
            {
                return View(_mapper.Map<UserViewModel>(userDetail));
            }

            this.StatusMessage = "Error can't find user for edit";
            return RedirectToAction(nameof(UsersController.Index));
        }

        [HttpGet]
        public IActionResult Create()
        {
            this.StatusMessage = "Cancel new user";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    if(user == null)
                    {
                        var newUser = _mapper.Map<SgsUser>(model);

                        var result = await _userManager.CreateAsync(newUser, model.Password);

                        if(result.Succeeded)
                        {
                            _logger.LogInformation("User created a new account with password by Admin.");
                            StatusMessage = "New User Created";
                            return RedirectToAction(nameof(UsersController.Index));
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Username", "user name alreadey exist !!");
                    }
                    
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("","Error happend");
                }
            }
            this.StatusMessage = "Cancel new user";
            return View(model);
        }

        public async Task<IActionResult> Edit(string username)
        {
            this.StatusMessage = "Cancel edit user";
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                this.StatusMessage = "Error can't find user for edit";
                return RedirectToAction(nameof(UsersController.Index));
            }

            return View(_mapper.Map<UserViewModel>(user));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(string username,UserViewModel model)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                this.StatusMessage = "Error can't find user for edit";
                return RedirectToAction(nameof(UsersController.Index));
            }

            if(ModelState.IsValid)
            {
                try
                {
                    var email = user.Email;
                    if (model.Email != email)
                    {
                        var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                        if (!setEmailResult.Succeeded)
                        {
                            throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                        }
                    }

                    var phoneNumber = user.PhoneNumber;
                    if (model.PhoneNumber != phoneNumber)
                    {
                        var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                        if (!setPhoneResult.Succeeded)
                        {
                            throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                        }
                    }

                    if (model.Name != user.Name)
                    {
                        user.Name = model.Name;
                        var setNameResult = await _userManager.UpdateAsync(user);
                        if (!setNameResult.Succeeded)
                        {
                            throw new ApplicationException($"Unexpected error occurred setting name for user with ID '{user.Id}'.");
                        }
                    }

                    if (user.EmployeeId != model.EmployeeId)
                    {
                        user.EmployeeId = model.EmployeeId;
                        var setEmployeeIdResult = await _userManager.UpdateAsync(user);
                        if (!setEmployeeIdResult.Succeeded)
                        {
                            throw new ApplicationException($"Unexpected error occurred setting employee id for user with ID '{user.Id}'.");
                        }
                    }

                    StatusMessage = "User profile has been updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(ex.Message, "Error happend");
                }
            }

            return View();
        }

        public async Task<IActionResult> ChangePassword(string username)
        {
            this.StatusMessage = "Cancel change user password !";
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                this.StatusMessage = "Error can't find user for edit";
                return RedirectToAction(nameof(UsersController.Index));
            }

            return View(new UserChangePasswordViewModel { Username = user.UserName });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string username, UserChangePasswordViewModel model)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                this.StatusMessage = "Error can't find user for chang password";
                return RedirectToAction(nameof(UsersController.Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Admin change user password");
                        StatusMessage = "Password changed";
                        return RedirectToAction(nameof(UsersController.Index));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(ex.Message, "Error happend");
                }
            }

            return View();
        }

        public async Task<IActionResult> ConfirmDelete(string userName)
        {
            this.StatusMessage = "Cancel delete user";
            if (User.Identity.Name == userName)
            {
                StatusMessage = "Error you can't delete your self !!";
                return RedirectToAction(nameof(UsersController.Index));
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                this.StatusMessage = "Error can't find user for delete";
                return RedirectToAction(nameof(UsersController.Index));
            }

            return View("Delete",_mapper.Map<UserViewModel>(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                this.StatusMessage = "Error can't find user for delete";
                return RedirectToAction(nameof(UsersController.Index));
            }

            var results = await _userManager.DeleteAsync(user);
            if (results.Succeeded)
            {
                _logger.LogInformation("User deleted by Admin.");
                StatusMessage = "User deleted successful !";
            }
            else
            {
                _logger.LogInformation("User can't deleted by Admin.");
                StatusMessage = "Delete error";
            }
            return RedirectToAction(nameof(UsersController.Index));
        }

        [AcceptVerbs("Get","Post")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyUsername(string username,string id="")
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user != null && user.Id.Trim().ToLower() != id?.Trim().ToLower())
                {
                    return Json($"Sorry {username} is already taken - عفواً اسم المستخدم مسجل مسبقاً");
                }
                return Json(true);
            }
            catch (Exception)
            {
                return Json("validation error ...!!");
            }
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public IActionResult VerifyEmployeeId(int employeeId, string id="")
        {
            try
            {
                var user = _userManager.Users.FirstOrDefault(u => u.EmployeeId == employeeId);
                if (user != null && user.Id.Trim().ToLower() != id?.Trim().ToLower())
                {
                    return Json($"Sorry {employeeId} is already registred - عفواً رقم الموظف مسجل مسبقاً");
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
