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
    public class CabinetController : BaseController
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            var user = await _userManager.GetUserAsync(User);
            var name = user.LastName + " " + user.FirstName;
            if (!(user.Patronymic is null))
                name += " " + user.Patronymic;
            var group = await _groupService.GetByIdAsync(user.GroupId);
            var flow = await _flowService.GetByIdAsync(group.FlowId);
            var faculty = await _facultyService.GetByIdAsync(flow.FacultyId);
            var groupName = flow.Name + group.Number;
            if (!(flow.Postfix is null))
                groupName += flow.Postfix;
            var votings = (await _votingService.GetUserVotingsAsync(UserId))
                .Select(async m => new VotingReducedViewModel()
                {
                    CreationDate = m.CreationDate,
                    ForPercentage = await _votingService.GetActualForPercentageAsync(m) * 100,
                    Id = m.Id,
                    Status = await _votingService.GetVotingStatusAsync(m),
                    IsSuccessfulNow = await _votingService.IsVotingSuccessfulNowAsync(m),
                    Name = m.Name,
                    Level = await _votingService.GetVotingLevelAsync(m)
                }).Select(t => t.Result); ;
            var profile = new UserProfileViewModel()
            {
                Name = name,
                Email = user.Email,
                TelegramTag = user.TelegramTag,
                Faculty = faculty.Name,
                Group = groupName,
                Roles = await _userManager.GetRolesAsync(user),
                Votings = votings
            };
            return View(profile);
        }
    }
}
