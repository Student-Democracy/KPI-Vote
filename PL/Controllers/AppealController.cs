using BLL.Interfaces;
using BLL.Models;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PL.Controllers
{
    [Authorize]
    public class AppealController : BaseController
    {
        private readonly IAppealService _appealService;
        private readonly UserManager<User> _userManager;

        public AppealController(
            IAppealService appealService,
            UserManager<User> userManager)
        {
            _appealService = appealService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vms = await GetAppealViewModelsAsync();
            return View(vms);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppealModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Message))
                ModelState.AddModelError("EmptyMessage", "Звернення не може бути порожнім.");
            else
                ViewBag.Message = model.Message;
            if (model.Importance <= 0)
                ModelState.AddModelError("WrongImportance", "Виберіть одну з тем звернення.");
            else
                ViewBag.Importance = model.Importance;
            if (ModelState.IsValid)
            {
                model.UserId = UserId;
                await _appealService.AddAsync(model);
                return RedirectToAction("Index");
            }
            else
            {
                var vms = await GetAppealViewModelsAsync();
                return View("Index", vms);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var appeal = await _appealService.GetByIdAsync(id);
            if (appeal==null||appeal.UserId != UserId)
                RedirectToAction("Index");
            return View(appeal);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AppealModel model)
        {
            var appeal = await _appealService.GetByIdAsync(model.Id);
            if (appeal == null)
                return RedirectToAction("Index");
            if (string.IsNullOrWhiteSpace(model.Message))
            {
                ModelState.AddModelError("EmptyMessage", "Звернення не може бути порожнім.");
            }
            if (model.Importance <= 0)
                ModelState.AddModelError("WrongImportance", "Виберіть одну з тем звернення.");
            if(!ModelState.IsValid)
                return View(appeal);
            appeal.Importance = model.Importance;
            appeal.Message = model.Message;
            await _appealService.UpdateAsync(appeal);
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public async Task<IActionResult> DeleteAppealByUser(int id)
        {
            var appeal = await _appealService.GetByIdAsync(id);
            if (appeal!=null&&appeal.UserId == UserId)
                await _appealService.DeleteByIdAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Адміністратор")]
        public async Task<IActionResult> DeleteAppealByAdmin(int id)
        {
            var appeal = await _appealService.GetByIdAsync(id);
            if(appeal!=null)
                await _appealService.DeleteByIdAsync(id);
            return RedirectToAction("UnresponsedAppeals");
        }

        [HttpGet]
        [Authorize(Roles = "Адміністратор")]
        public async Task<IActionResult> UnresponsedAppeals()
        {
            var appeals = await _appealService.GetUnresponsedAppealsAsync();
            var vms = new List<AppealViewModel>();
            foreach (var appeal in appeals)
            {
                vms.Add(new AppealViewModel()
                {
                    Id = appeal.Id,
                    Message = appeal.Message,
                    User = (await _userManager.FindByIdAsync(appeal.UserId)).UserName,
                    Importance = appeal.Importance,
                    CreationDate = appeal.Date
                });
            }
            return View(vms);
        }

        [HttpGet]
        [Authorize(Roles = "Адміністратор")]
        public async Task<IActionResult> RespondAppeal(int id)
        {
            var appeal = await _appealService.GetByIdAsync(id);
            if (appeal == null)
                return RedirectToAction("UnresponsedAppeals");
            var appealVm = new AppealViewModel
            {
                Id = appeal.Id,
                Message = appeal.Message,
                CreationDate = appeal.Date,
                User = (await _userManager.FindByIdAsync(appeal.UserId)).UserName
            };
            return View(appealVm);
        }

        [HttpPost]
        [Authorize(Roles = "Адміністратор")]
        public async Task<IActionResult> RespondAppeal(AppealModel model)
        {
            var appeal = await _appealService.GetByIdAsync(model.Id);
            if (appeal == null || appeal.Response!=null)
                return RedirectToAction("UnresponsedAppeals");
            if (string.IsNullOrWhiteSpace(model?.Response))
            {
                ModelState.AddModelError("EmptyResponse", "Відповідь не може бути порожньою.");
                var vm = new AppealViewModel
                {
                    User = (await _userManager.FindByIdAsync(appeal.UserId)).UserName,
                    Message = appeal.Message,
                    CreationDate = appeal.Date
                };
                return View(vm);
            }
            appeal.Response = model.Response;
            appeal.AdminId = UserId;
            await _appealService.ResponseAppealAsync(appeal);
            return RedirectToAction("UnresponsedAppeals");
        }

        private async Task<IEnumerable<AppealViewModel>> GetAppealViewModelsAsync()
        {
            var appeals = await _appealService.GetUserAppealsAsync(UserId);
            var vms = new List<AppealViewModel>();
            foreach (var appeal in appeals)
            {

                string admin;
                if (appeal.AdminId != null)
                    admin = (await _userManager.FindByIdAsync(appeal.AdminId)).UserName;
                else admin = null;
                vms.Add(new AppealViewModel()
                {
                    Id = appeal.Id,
                    Message = appeal.Message,
                    Response = appeal.Response,
                    Admin = admin,
                    User = _userManager.GetUserName(User),
                    CreationDate = appeal.Date
                });
            }
            return vms;
        }
    }
}
