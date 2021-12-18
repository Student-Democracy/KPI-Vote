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

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager = null)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
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
    }
}
