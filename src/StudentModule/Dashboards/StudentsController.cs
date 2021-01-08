using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SatelliteSite.StudentModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.Entities;
using Tenant.Services;

namespace SatelliteSite.StudentModule.Dashboards
{
    [Area("Dashboard")]
    [Authorize(Roles = "Administrator,Teacher")]
    [Route("[area]/[controller]")]
    [AuditPoint(AuditlogType.Student)]
    public class StudentsController : TenantControllerBase
    {
        private const int ItemPerPage = 50;
        private IStudentStore Store { get; }
        private IUserManager UserManager { get; }

        public StudentsController(IStudentStore store, IUserManager users)
        {
            Store = store;
            UserManager = users;
        }


        [HttpGet]
        public async Task<IActionResult> List(int page = 1)
        {
            if (page < 1) page = 1;
            var model = await Store.ListStudentsAsync(Affiliation, page, ItemPerPage);
            return View(model);
        }


        [HttpGet("{stuid}/[action]")]
        public async Task<IActionResult> Delete(int page, string stuid)
        {
            var stuId = await Store.FindStudentAsync(Affiliation, stuid);
            if (stuId == null) return NotFound();

            return AskPost(
                title: $"Delete student {stuid}",
                message: $"Are you sure to delete student {stuid} - {stuId.Name}?",
                area: "Dashboard", controller: "Students", action: "Delete",
                routeValues: new { page },
                type: BootstrapColor.warning);
        }


        [HttpPost("{stuid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int page, string stuid, bool post = true)
        {
            var stud = await Store.FindStudentAsync(Affiliation, stuid);
            if (stud == null) return NotFound();

            var users = await Store.FindUserByStudentAsync(stud);
            string unlinked = "";

            foreach (var user in users)
            {
                unlinked += $" u{user.Id}";
                ((IUserWithStudent)user).StudentEmail = null;
                ((IUserWithStudent)user).StudentId = null;
                ((IUserWithStudent)user).StudentVerified = false;
                await UserManager.UpdateAsync(user);
                await UserManager.RemoveFromRoleAsync(user, "Student");
            }

            await Store.DeleteAsync(stud);
            await HttpContext.AuditAsync("deleted", $"student s{stuid}", string.IsNullOrEmpty(unlinked) ? null : $"unlink{unlinked}");
            StatusMessage = $"Student ID {stuid} has been removed.";
            return RedirectToAction(nameof(List), new { page });
        }


        [HttpGet("[action]")]
        public IActionResult Add()
        {
            return Window(new BatchAddModel());
        }


        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(BatchAddModel model)
        {
            var stus = model.Batch.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var adds = new Dictionary<string, string>();

            foreach (var item in stus)
            {
                var ofs = item.Trim().Split(new[] { ' ', '\t', ',' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (ofs.Length != 2)
                    ModelState.AddModelError("xys::parseError2", $"Unrecognized token {item.Trim()}.");
                if (!long.TryParse(ofs[0], out long _))
                    ModelState.AddModelError("xys::parseError", $"Unrecognized number token {ofs[0]}.");
                else if (adds.ContainsKey(ofs[0]))
                    ModelState.AddModelError("xys::parseError3", $"Duplicate student id {ofs[0]}.");
                else
                    adds.Add(ofs[0], ofs[1]);
            }

            if (!ModelState.IsValid) return Window(model);

            int rows = await Store.MergeAsync(Affiliation, adds);
            await HttpContext.AuditAsync("merge", "students");
            StatusMessage = $"{rows} students updated or added.";
            return RedirectToAction(nameof(List), new { page = 1 });
        }


        [HttpGet("{stuid}/[action]/{uid}")]
        public async Task<IActionResult> Unlink(int page, string stuid, int uid)
        {
            var stud = await Store.FindStudentAsync(Affiliation, stuid);
            var users = await Store.FindUserByStudentAsync(stud);
            var user = users.SingleOrDefault(u => u.Id == uid);
            if (user == null) return NotFound();

            return AskPost(
                title: $"Unlink student {stuid}",
                message: $"Are you sure to unlink student {stuid} with {user.UserName} (u{user.Id})?",
                area: "Dashboard", controller: "Students", action: "Unlink",
                routeValues: new { page },
                type: BootstrapColor.warning);
        }


        [HttpPost("{stuid}/[action]/{uid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlink(int page, string stuid, int uid, bool post = true)
        {
            var stud = await Store.FindStudentAsync(Affiliation, stuid);
            var users = await Store.FindUserByStudentAsync(stud);
            var user = users.SingleOrDefault(u => u.Id == uid);
            if (user == null) return NotFound();
            var user2 = (IUserWithStudent)user;

            user2.StudentEmail = null;
            user2.StudentId = null;
            user2.StudentVerified = false;
            await UserManager.UpdateAsync(user);
            await UserManager.RemoveFromRoleAsync(user, "Student");
            StatusMessage = $"Student ID {stuid} has been unlinked with u{user.Id}.";
            await HttpContext.AuditAsync("unlinked", $"u{user.Id}", user == null ? null : $"student s{stuid}");
            return RedirectToAction(nameof(List), new { page });
        }


        [HttpGet("{stuid}/[action]/{uid}")]
        public async Task<IActionResult> MarkVerified(int page, string stuid, int uid)
        {
            var stud = await Store.FindStudentAsync(Affiliation, stuid);
            if (stud == null) return NotFound();

            var users = await Store.FindUserByStudentAsync(stud);
            var user = users.SingleOrDefault(u => u.Id == uid);
            if (user == null) return NotFound();
            var user2 = (IUserWithStudent)user;
            
            if (!user2.StudentVerified)
            {
                user2.StudentVerified = true;
                await UserManager.UpdateAsync(user);
                await UserManager.AddToRoleAsync(user, "Student");
                await HttpContext.AuditAsync("verified", $"student s{stuid}", user == null ? null : $"to u{user.Id}");
            }

            StatusMessage = $"Marked {user.UserName} (u{user.Id}) as verified student.";
            return RedirectToAction(nameof(List), new { page });
        }
    }
}
