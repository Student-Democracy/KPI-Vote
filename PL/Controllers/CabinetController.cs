using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DAL.Entities;
using PL.Models;
using BLL.Services;
using BLL.Interfaces;

namespace PL.Controllers
{
    [Authorize]
    public class CabinetController : Controller
    {
        private readonly UserManager<User> _userManager;

        private readonly IGroupService _groupService;

        private readonly IFlowService _flowService;

        private readonly IFacultyService _facultyService;

        private readonly IVotingService _votingService;

        public CabinetController(UserManager<User> userManager, IGroupService groupService, IFlowService flowService, IFacultyService facultyService, IVotingService votingService)
        {
            _userManager = userManager;
            _groupService = groupService;
            _flowService = flowService;
            _facultyService = facultyService;
            _votingService = votingService;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            var user = await _userManager.GetUserAsync(User);
            var group = await _groupService.GetByIdAsync(user.GroupId);
            var flow = await _flowService.GetByIdAsync(group.FlowId);
            var faculty = await _facultyService.GetByIdAsync(flow.FacultyId);
            var groupName = flow.Name + group.Number;
            if (!(flow.Postfix is null))
                groupName += flow.Postfix;
            var votings = await _votingService.GetUserVotingsAsync(user.Id);
            var profile = new UserProfileViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Patronymic = user.Patronymic,
                Email = user.Email,
                TelegramTag = user.TelegramTag,
                Faculty = faculty.Name,
                Group = groupName,
                //Votings = Добавить!!!
            };
            return View(profile);
        }
    }
}
