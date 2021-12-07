using BLL.Interfaces;
using BLL.Models;
using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        public VotingsController(IVotingService votingService)
        {
            _votingService = votingService;
        }

        // GET: VotingsController
        [HttpGet]
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
        public IActionResult Details(int id)
        {
            return View();
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
