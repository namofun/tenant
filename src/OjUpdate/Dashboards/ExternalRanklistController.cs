﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SatelliteSite.Entities;
using SatelliteSite.OjUpdateModule.Entities;
using SatelliteSite.OjUpdateModule.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelliteSite.OjUpdateModule.Dashboards
{
    [Area("Dashboard")]
    [Authorize(Roles = "Administrator,Teacher")]
    [Route("[area]/[controller]")]
    [AuditPoint(AuditlogType.User)]
    public class ExternalRanklistController : ViewControllerBase
    {
        const int ItemsPerPage = 50;

        private ISolveRecordStore Store { get; }

        public ExternalRanklistController(ISolveRecordStore store)
        {
            Store = store;
        }


        [HttpGet]
        public async Task<IActionResult> List(int page = 1)
        {
            if (page <= 0) return BadRequest();
            var model = await Store.ListAsync(page, ItemsPerPage);
            return View(model);
        }


        [HttpGet("[action]")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Create(
            [FromForm] SolveRecord[] Batch)
        {
            var results = new List<SolveRecord>();
            foreach (var item in Batch)
            {
                if (string.IsNullOrWhiteSpace(item.Account) ||
                    string.IsNullOrWhiteSpace(item.NickName))
                    continue;
                item.Id = 0;
                item.Result = null;
                results.Add(item);
            }

            var tot = await Store.CreateAsync(results);
            StatusMessage = $"Successfully added {results.Count} items!";
            return RedirectToAction(nameof(List), new { page = (tot - 1) / ItemsPerPage + 1 });
        }


        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Delete(int id, int page)
        {
            var item = await Store.FindAsync(id);
            if (item == null) return NotFound();
            return AskPost(
                title: $"Delete record {id}",
                message: $"Are you sure to remove {item.NickName} - {item.Category}, {item.Grade}?",
                routeValues: new { id, page },
                type: BootstrapColor.danger);
        }


        [HttpPost("{id}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int page, bool post = true)
        {
            var item = await Store.FindAsync(id);
            if (item == null) return NotFound();
            await Store.DeleteAsync(item);
            StatusMessage = "Successfully deleted.";
            return RedirectToAction(nameof(List), new { page });
        }


        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Edit(int id, int page)
        {
            var item = await Store.FindAsync(id);
            if (item == null) return NotFound();
            ViewBag.Page = page;
            return Window(item);
        }


        [HttpPost("{id}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int page, SolveRecord model)
        {
            var item = await Store.FindAsync(id);
            if (item == null) return NotFound();

            item.Account = model.Account;
            item.Category = model.Category;
            item.Grade = model.Grade;
            item.NickName = model.NickName;

            await Store.UpdateAsync(item);
            StatusMessage = "Successfully updated.";
            return RedirectToAction(nameof(List), new { page });
        }
    }
}
