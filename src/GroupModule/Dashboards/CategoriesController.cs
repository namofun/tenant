using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SatelliteSite.GroupModule.Models;
using System.Threading.Tasks;
using Tenant.Entities;
using Tenant.Services;

namespace SatelliteSite.GroupModule.Dashboards
{
    [Area("Dashboard")]
    [Authorize(Roles = "Administrator")]
    [Route("[area]/[controller]")]
    [AuditPoint(AuditlogType.TeamCategory)]
    public class CategoriesController : ViewControllerBase
    {
        private ICategoryStore Store { get; }
        public CategoriesController(ICategoryStore tm) => Store = tm;


        [HttpGet]
        public async Task<IActionResult> List()
        {
            return View(await Store.ListAsync());
        }


        [HttpGet("{catid}")]
        public async Task<IActionResult> Detail(int catid)
        {
            var cat = await Store.FindAsync(catid);
            if (cat == null) return NotFound();
            return View(cat);
        }


        [HttpGet("[action]")]
        public IActionResult Add()
        {
            return View("Edit", new CategoryModel());
        }


        [HttpGet("{catid}/[action]")]
        public async Task<IActionResult> Edit(int catid)
        {
            var cat = await Store.FindAsync(catid);
            if (cat == null) return NotFound();

            return View(new CategoryModel
            {
                Id = cat.Id,
                Color = cat.Color,
                SortOrder = cat.SortOrder,
                IsPublic = cat.IsPublic,
                Name = cat.Name,
            });
        }


        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CategoryModel model)
        {
            var e = await Store.CreateAsync(new Category
            {
                Color = model.Color,
                SortOrder = model.SortOrder,
                IsPublic = model.IsPublic,
                Name = model.Name,
            });

            await HttpContext.AuditAsync("created", $"{e.Id}");
            return RedirectToAction(nameof(Detail), new { catid = e.Id });
        }


        [HttpPost("{catid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int catid, CategoryModel model)
        {
            var cat = await Store.FindAsync(catid);
            if (cat == null) return NotFound();

            if (!ModelState.IsValid) return View(model);
            cat.Color = model.Color;
            cat.IsPublic = model.IsPublic;
            cat.Name = model.Name;
            cat.SortOrder = model.SortOrder;

            await Store.UpdateAsync(cat);
            await HttpContext.AuditAsync("updated", $"{catid}");
            return RedirectToAction(nameof(Detail), new { catid });
        }


        [HttpGet("{catid}/[action]")]
        public async Task<IActionResult> Delete(int catid)
        {
            var desc = await Store.FindAsync(catid);
            if (desc == null) return NotFound();

            return AskPost(
                title: $"Delete team category {catid} - \"{desc.Name}\"",
                message: $"You're about to delete team category {catid} - \"{desc.Name}\".\n" +
                    "Are you sure?",
                area: "Dashboard", controller: "Categories", action: "Delete",
                type: BootstrapColor.danger);
        }


        [HttpPost("{catid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int catid, int inajax)
        {
            var desc = await Store.FindAsync(catid);
            if (desc == null) return NotFound();

            try
            {
                await Store.DeleteAsync(desc);
                StatusMessage = $"Team category {catid} deleted successfully.";
                await HttpContext.AuditAsync("deleted", $"{catid}");
            }
            catch
            {
                StatusMessage = $"Error deleting team category {catid}, foreign key constraints failed.";
            }

            return RedirectToAction(nameof(List));
        }
    }
}
