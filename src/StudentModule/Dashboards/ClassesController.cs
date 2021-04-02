using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SatelliteSite.StudentModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenant.Services;

namespace SatelliteSite.StudentModule.Dashboards
{
    [Area("Dashboard")]
    [Authorize(Policy = "TenantAdmin")]
    [Route("[area]/[controller]")]
    public class ClassesController : TenantControllerBase
    {
        private const int ItemPerPage = 50;
        private IStudentStore Store { get; }
        public ClassesController(IStudentStore store) => Store = store;


        [HttpGet]
        public async Task<IActionResult> List()
        {
            var model = await Store.ListClassesAsync(Affiliation);
            return View(model);
        }


        [HttpGet("{clsid}/[action]")]
        public async Task<IActionResult> Delete(int clsid)
        {
            var model = await Store.FindClassAsync(Affiliation, clsid);
            if (model == null) return NotFound();

            return AskPost(
                title: $"Delete group {clsid}",
                message: $"Are you sure to delete group {model.Name}?",
                area: "Dashboard", controller: "Classes", action: "Delete",
                type: BootstrapColor.danger);
        }


        [HttpPost("{clsid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int clsid, bool post = true)
        {
            var @class = await Store.FindClassAsync(Affiliation, clsid);
            if (@class == null) return NotFound();

            await Store.DeleteAsync(@class);
            return RedirectToAction(nameof(List));
        }


        [HttpGet("{clsid}")]
        public async Task<IActionResult> Detail(int clsid, int page = 1)
        {
            var model = await Store.FindClassAsync(Affiliation, clsid);
            if (model == null) return NotFound();

            if (page < 1) return BadRequest();
            ViewBag.Students = await Store.ListStudentsAsync(model, page, 200);
            return View(model);
        }


        [HttpGet("{clsid}/[action]")]
        public async Task<IActionResult> Clone(int clsid)
        {
            var model = await Store.FindClassAsync(Affiliation, clsid);
            if (model == null) return NotFound();
            ViewBag.Class = model;

            return Window(new CreateClassModel());
        }


        [HttpPost("{clsid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clone(int clsid, CreateClassModel model)
        {
            var old = await Store.FindClassAsync(Affiliation, clsid);
            if (old == null) return NotFound();

            var userId = !model.IsShared ? int.Parse(User.GetUserId()) : default(int?);
            var @new = await Store.CloneAsync(old, model.ClassName, userId, User.GetUserName());
            return RedirectToAction(nameof(Detail), new { clsid = @new.Id });
        }


        [HttpGet("[action]")]
        public IActionResult Create()
        {
            return Window(new CreateClassModel());
        }


        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateClassModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ClassName))
                ModelState.AddModelError("model", "Class Name should not be empty.");
            if (!ModelState.IsValid) return Window(model);

            var userId = !model.IsShared ? int.Parse(User.GetUserId()) : default(int?);
            var cls = await Store.CreateAsync(Affiliation, model.ClassName, userId, User.GetUserName());
            return RedirectToAction(nameof(Detail), new { clsid = cls.Id });
        }


        [HttpGet("{clsid}/[action]")]
        public async Task<IActionResult> Add(int clsid)
        {
            var model = await Store.FindClassAsync(Affiliation, clsid);
            if (model == null) return NotFound();
            return Window(new BatchAddModel());
        }


        [HttpPost("{clsid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int clsid, BatchAddModel model)
        {
            var @class = await Store.FindClassAsync(Affiliation, clsid);
            if (@class == null) return NotFound();

            var stus = model.Batch.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            var opts = new List<string>();

            foreach (var item in stus)
            {
                if (!long.TryParse(item.Trim(), out long stuid))
                {
                    ModelState.AddModelError("xys::parseError", $"Unrecognized number token {item}.");
                    sb.Append(item.Trim()).Append('\n');
                }
                else
                {
                    opts.Add(item.Trim());
                }
            }

            var intersects = await Store.CheckExistingStudentsAsync(Affiliation, opts);

            foreach (var stuId in opts.Except(intersects))
            {
                sb.Append($"{stuId}\n");
                ModelState.AddModelError("xys::parseError", $"Student {stuId} not found.");
            }

            await Store.MergeAsync(@class, intersects);

            if (ModelState.IsValid)
            {
                StatusMessage = $"{intersects.Count} students has been added.";
                return RedirectToAction(nameof(Detail));
            }
            else
            {
                ModelState.SetModelValue("Batch", sb.ToString(), sb.ToString());
                model.Batch = sb.ToString();
                ModelState.AddModelError("xys::other", $"Other {intersects.Count} students has been added.");
                return Window(model);
            }
        }


        [HttpGet("{clsid}/[action]/{stuid}")]
        public async Task<IActionResult> Kick(int clsid, string stuid)
        {
            var model = await Store.FindClassAsync(Affiliation, clsid);
            var stud = await Store.FindStudentAsync(Affiliation, stuid);
            if (model == null || stud == null) return NotFound();

            return AskPost(
                title: $"Kick student from group",
                message: $"Are you sure to kick student {stuid} from group {model.Name}?",
                area: "Dashboard", controller: "Classes", action: "Kick",
                type: BootstrapColor.warning);
        }


        [HttpPost("{clsid}/[action]/{stuid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Kick(int clsid, string stuid, bool post = true)
        {
            var @class = await Store.FindClassAsync(Affiliation, clsid);
            var stud = await Store.FindStudentAsync(Affiliation, stuid);

            if (await Store.KickAsync(@class, stud))
                StatusMessage = $"Kicked student {stuid} from group g{clsid}.";
            else
                StatusMessage = "Error occurred when kicking student.";
            return RedirectToAction(nameof(Detail));
        }
    }
}
