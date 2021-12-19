using BLL.Interfaces;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;

        private readonly UserManager<User> _userManager;

        private readonly IBlockService _blockService;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, IBlockService blockService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _blockService = blockService;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var user = await _userManager.GetUserAsync(User);
            if (!(user is null))
                return RedirectToAction("Index", "Cabinet");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!(user is null))
                return RedirectToAction("Index", "Cabinet");
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Cabinet");
                }
                else
                {
                    ModelState.AddModelError("LoginError", "Неправильна електронна пошта та/або пароль");
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LogoutConfirmed()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangeEmail()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                user.UserName = model.NewEmail;
                user.NormalizedUserName = model.NewEmail.ToUpperInvariant();
                user.Email = model.NewEmail;
                user.NormalizedEmail = user.NormalizedUserName;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Cabinet");
                else
                    ModelState.AddModelError("EmailIsTakenError", "Така електронна пошта вже зайнята");
            }
            else
            {
                ModelState.AddModelError("PasswordError", "Неправильний пароль");
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (model.NewPassword == model.NewPasswordConfirm)
            {
                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    ModelState.AddModelError("PasswordError", "Неправильний пароль");
                }
                else if (model.Password == model.NewPassword)
                {
                    ModelState.AddModelError("NewPasswordError", "Старий пароль та новий не повинні збігатися");
                }
                else
                {
                    var result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
                    user = await _userManager.GetUserAsync(User);
                    if (!user.PasswordChanged)
                    {
                        user.PasswordChanged = true;
                        await _userManager.UpdateAsync(user);
                    }

                    if (result.Succeeded)
                    {
                        TempData["Message"] = "Пароль було змінено успішно";
                        return RedirectToAction("Index", "Cabinet");
                    }
                    else
                        ModelState.AddModelError("NewPasswordError", "Пароль неналежного формату (принаймні, 8 символів, з яких 1 цифра, 1 велика та 1 мала літери)");
                }
            }
            else
            {
                ModelState.AddModelError("NewPasswordConfirmError", "Новий пароль та його підтвердження відрізняються");
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Banned()
        {
            var user = await _userManager.GetUserAsync(User);
            var ban = await _blockService.GetByUserIdAsync(user.Id);
            if (ban is null)
                return RedirectToAction("Index", "Cabinet");
            await _signInManager.SignOutAsync();
            var admin = await _userManager.FindByIdAsync(ban.AdminId);
                var adminName = admin.LastName + " " + admin.FirstName;
                if (!(admin.Patronymic is null))
                    adminName += " " + admin.Patronymic;
                var model = new BanReducedViewModel()
                {
                    DateTo = ban.DateTo,
                    Hammer = ban.Hammer,
                    AdminEmail = admin.Email,
                    AdminTelegramTag = admin.TelegramTag,
                    AdminName = adminName
                };
            return View(model);
        }
    }
}
