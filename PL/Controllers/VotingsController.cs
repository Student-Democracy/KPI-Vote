using BLL.Interfaces;
using BLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PL.Controllers
{
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
            var model = await _votingService.GetFilteredAndSortedForUserAsync(UserId);
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
