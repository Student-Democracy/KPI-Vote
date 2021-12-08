using BLL.Interfaces;
using BLL.Models;
using BLL.Services;
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
    [Authorize]
    public class VotingsController : BaseController
    {
        private readonly IVotingService _votingService;

        private readonly IVoteService _voteService;

        private readonly UserManager<User> _userManager;

        private readonly IGroupService _groupService;

        private readonly IFlowService _flowService;

        private readonly IFacultyService _facultyService;

        public VotingsController(IVotingService votingService, UserManager<User> userManager, IVoteService voteService, IGroupService groupService, IFlowService flowService, IFacultyService facultyService)
        {
            _votingService = votingService;
            _userManager = userManager;
            _voteService = voteService;
            _groupService = groupService;
            _flowService = flowService;
            _facultyService = facultyService;
        }

        // GET: VotingsController
        [HttpGet]
        [Route("Votings")]
        public async Task<IActionResult> Index(string votingstype)
        {
            var user = await _userManager.GetUserAsync(User);
            if (votingstype is null)
                return RedirectToAction("Index", new { votingstype = "actual" });
            else if (votingstype == "all" && !await _userManager.IsInRoleAsync(user, "Адміністратор"))
                return RedirectToAction("Index", new { votingstype = "actual" });
            else if ((votingstype == "requests" || votingstype == "checked")
                && !(await _userManager.IsInRoleAsync(user, "Адміністратор")
                || await _userManager.IsInRoleAsync(user, "Голова СР КПІ")
                || await _userManager.IsInRoleAsync(user, "Голова СР Факультету")
                || await _userManager.IsInRoleAsync(user, "Староста потоку")
                || await _userManager.IsInRoleAsync(user, "Староста групи")))
                return RedirectToAction("Index", new { votingstype = "actual" });
            ViewBag.votingstype = votingstype;
            IEnumerable<VotingModel> tempModel = null;
            tempModel = votingstype switch
            {
                "all" => await _votingService.GetFilteredAndSortedForAdminAsync(),
                "requests" => (await _votingService.GetNotConfirmedAsync()).Where(v => IsUserAbleToChangeStatusAsync(v).Result),
                "checked" => _votingService.GetAll().Where(v => v.StatusSetterId == UserId),
                "created" => _votingService.GetAll().Where(v => v.AuthorId == UserId),
                _ => await _votingService.GetFilteredAndSortedForUserAsync(UserId),
            };
            ;
            var model = tempModel.Select(async m => new VotingReducedViewModel()
            {
                CreationDate = m.CreationDate,
                ForPercentage = await _votingService.GetActualForPercentageAsync(m) * 100,
                Id = m.Id,
                Status = await _votingService.GetVotingStatusAsync(m),
                IsSuccessfulNow = await _votingService.IsVotingSuccessfulNowAsync(m),
                Name = m.Name,
                Level = await _votingService.GetVotingLevelAsync(m),
                IsUserAbleToChangeStatus = await IsUserAbleToChangeStatusAsync(m)
            }).Select(t => t.Result);
            return View(model);
        }

        // GET: VotingsController/Details/5
        [HttpGet]
        [Route("Votings/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _votingService.GetByIdAsync(id);
            VotingViewModel mappedModel = null;
            if (model != null && await CanUserViewVotingAsync(model))
            {
                mappedModel = new VotingViewModel()
                {
                    Id = model.Id,
                    CreationDate = model.CreationDate,
                    CompletionDate = model.CompletionDate,
                    Status = await _votingService.GetVotingStatusAsync(model),
                    Name = model.Name,
                    Description = model.Description,
                    IsSuccessfulNow = await _votingService.IsVotingSuccessfulNowAsync(model),
                    Level = await _votingService.GetVotingLevelAsync(model),
                    UserVote = null,
                    MinimalAttendancePercentage = model.MinimalAttendancePercentage,
                    MinimalForPercentage = model.MinimalForPercentage,
                    AttendancePercentage = await _votingService.GetActualAttendancePercentageAsync(model) * 100,
                    VotesFor = await _votingService.GetVotersForNumberAsync(model),
                    VotesTotally = await _votingService.GetVotersNumberAsync(model),
                    AuthorId = model.AuthorId,
                    StatusSetterId = model.StatusSetterId,
                    IsUserAbleToVote = await IsUserAbleToVoteAsync(model),
                    IsUserAbleToChangeStatus = await IsUserAbleToChangeStatusAsync(model)
                };
                var author = await _userManager.FindByIdAsync(mappedModel.AuthorId);
                var statusSetter = await _userManager.FindByIdAsync(mappedModel.StatusSetterId);
                var authorName = author.LastName + " " + author.FirstName;
                if (!(author.Patronymic is null))
                    authorName += " " + author.Patronymic;
                var statusSetterName = statusSetter.LastName + " " + statusSetter.FirstName;
                if (!(statusSetter.Patronymic is null))
                    statusSetterName += " " + statusSetter.Patronymic;
                mappedModel.Author = authorName;
                mappedModel.StatusSetter = statusSetterName;
                var userVote = _voteService.GetAll().Where(v => v.UserId == UserId && v.VotingId == model.Id).SingleOrDefault();
                if (!(userVote is null))
                {
                    mappedModel.UserVote = userVote.Result switch
                    {
                        VoteResult.For => "ЗА",
                        VoteResult.Against => "ПРОТИ" ,
                        _ => "Нейтрально"
                    };
                }    
            }
            else
            {
                ModelState.AddModelError("VotingNotFoundError", "Такого голосування не знайдено або ви не маєте до нього доступу");
            }
            return View(mappedModel);
        }

        private async Task<bool> CanUserViewVotingAsync(VotingModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var group = await _groupService.GetByIdAsync(user.GroupId);
            var flow = await _flowService.GetByIdAsync(group.FlowId);
            var faculty = await _facultyService.GetByIdAsync(flow.FacultyId);
            if (await _userManager.IsInRoleAsync(user, "Адміністратор"))
                return true;
            else if (model.GroupId == group.Id || model.FlowId == flow.Id || model.FacultyId == faculty.Id
                || (model.GroupId is null && model.FlowId is null && model.FacultyId is null))
                return true;
            else if (await _userManager.IsInRoleAsync(user, "Голова СР КПІ") && !(model.FacultyId is null))
                return true;
            else if (await _userManager.IsInRoleAsync(user, "Голова СР Факультету") && !(model.FlowId is null) && (await _flowService.GetByIdAsync((int)model.FlowId)).FacultyId == faculty.Id)
                return true;
            else if (await _userManager.IsInRoleAsync(user, "Староста потоку") && !(model.GroupId is null) && (await _groupService.GetByIdAsync((int)model.GroupId)).FlowId == flow.Id)
                return true;
            return false;
        }

        private async Task<bool> IsUserAbleToVoteAsync(VotingModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var group = await _groupService.GetByIdAsync(user.GroupId);
            var flow = await _flowService.GetByIdAsync(group.FlowId);
            var faculty = await _facultyService.GetByIdAsync(flow.FacultyId);
            if (model.GroupId == group.Id || model.FlowId == flow.Id || model.FacultyId == faculty.Id
                || (model.GroupId is null && model.FlowId is null && model.FacultyId is null))
                return true;
            return false;
        }

        private async Task<bool> IsUserAbleToChangeStatusAsync(VotingModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var group = await _groupService.GetByIdAsync(user.GroupId);
            var flow = await _flowService.GetByIdAsync(group.FlowId);
            var faculty = await _facultyService.GetByIdAsync(flow.FacultyId);
            if (await _userManager.IsInRoleAsync(user, "Адміністратор"))
                return true;
            if (await _userManager.IsInRoleAsync(user, "Голова СР КПІ"))
                return !(model.FacultyId is null) || (model.FacultyId is null && model.FlowId is null && model.GroupId is null);
            else if (await _userManager.IsInRoleAsync(user, "Голова СР Факультету"))
                return model.FacultyId == faculty.Id || (!(model.FlowId is null) && _flowService.GetAll().Where(f => f.Id == model.FlowId).SingleOrDefault().FacultyId == faculty.Id);
            else if (await _userManager.IsInRoleAsync(user, "Староста потоку"))
                return model.FlowId == flow.Id || (!(model.GroupId is null) && _groupService.GetAll().Where(g => g.Id == model.GroupId).SingleOrDefault().FlowId == flow.Id);
            else if (await _userManager.IsInRoleAsync(user, "Староста групи"))
                return model.GroupId == group.Id;
            return false;
        }

        [HttpPost]
        [Route("Votings/{id}")]
        public async Task<IActionResult> Vote(int id, VotingViewModel model)
        {
            var votingModel = await _votingService.GetByIdAsync(id);
            if (!(votingModel is null) && await IsUserAbleToVoteAsync(votingModel))
            {
                try
                {
                    var voteModel = new VoteModel()
                    {
                        VotingId = id,
                        UserId = UserId,
                        Result = model.UserVote switch
                        {
                            "ЗА" => VoteResult.For,
                            "ПРОТИ" => VoteResult.Against,
                            _ => VoteResult.Neutral
                        }
                    };
                    await _voteService.AddAsync(voteModel);

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("VoteError", exc.Message);
                }
            }
            else
            {
                ModelState.AddModelError("VoteError", "Ви не можете проголосувати, тому що не належите до цієї групи/потоку/факультету");
            }
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [Route("Votings/Block")]
        public async Task<IActionResult> Block(int id)
        {
            var refferer = Request.Headers["Referer"].ToString();
            var voting = await _votingService.GetByIdAsync(id);
            if (!(voting is null) && await IsUserAbleToChangeStatusAsync(voting))
            {
                voting.Status = VotingStatus.Denied;
                voting.StatusSetterId = UserId;
                await _votingService.ChangeStatusAsync(voting);
            }
            else
            {
                ModelState.AddModelError("StatusChangeError", "Ви не можете змінити статус цього голосування");
            }
            return Redirect(refferer);
        }

        [HttpPost]
        [Route("Votings/Approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var refferer = Request.Headers["Referer"].ToString();
            var voting = await _votingService.GetByIdAsync(id);
            if (!(voting is null) && await IsUserAbleToChangeStatusAsync(voting))
            {
                voting.Status = VotingStatus.Confirmed;
                voting.StatusSetterId = UserId;
                await _votingService.ChangeStatusAsync(voting);
            }
            else
            {
                ModelState.AddModelError("StatusChangeError", "Ви не можете змінити статус цього голосування");
            }
            return Redirect(refferer);
        }

        // GET: VotingsController/Create
        [HttpGet]
        [Route("Votings/Create")]
        public async Task<IActionResult> Create()
        {
            var p = Request.Headers["Referer"].ToString();
            return View();
        }

        // POST: VotingsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Votings/Create")]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: VotingsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: VotingsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: VotingsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VotingsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
