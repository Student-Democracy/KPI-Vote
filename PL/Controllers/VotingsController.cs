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

        public VotingsController(IVotingService votingService, UserManager<User> userManager, IVoteService voteService)
        {
            _votingService = votingService;
            _userManager = userManager;
            _voteService = voteService;
        }

        // GET: VotingsController
        [HttpGet]
        [Route("Votings")]
        public async Task<IActionResult> Index()
        {
            IEnumerable<VotingReducedViewModel> model = (await _votingService.GetFilteredAndSortedForUserAsync(UserId))
                .Select(async m => new VotingReducedViewModel()
                {
                    CreationDate = m.CreationDate,
                    ForPercentage = await _votingService.GetActualForPercentageAsync(m) * 100,
                    Id = m.Id,
                    Status = await _votingService.GetVotingStatusAsync(m),
                    IsSuccessfulNow = await _votingService.IsVotingSuccessfulNowAsync(m),
                    Name = m.Name,
                    Level = await _votingService.GetVotingLevelAsync(m)
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
            if (model != null)
            {
                mappedModel = new VotingViewModel()
                {
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
                ModelState.AddModelError("VotingNotFoundError", "Такого голосування не знайдено");
            }
            return View(mappedModel);
        }

        [HttpPost]
        [Route("Votings/{id}")]
        public async Task<IActionResult> Vote(int id, VotingViewModel model)
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
            return RedirectToAction("Details", new { id });
        }

        // GET: VotingsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VotingsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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
