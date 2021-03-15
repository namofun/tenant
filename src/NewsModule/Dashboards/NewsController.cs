using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SatelliteSite.NewsModule.Entities;
using SatelliteSite.NewsModule.Models;
using SatelliteSite.NewsModule.Services;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SatelliteSite.NewsModule.Dashboards
{
    [Area("Dashboard")]
    [Authorize(Roles = "Administrator")]
    [Route("[area]/[controller]")]
    [AuditPoint(AuditlogType.News)]
    public class NewsController : ViewControllerBase
    {
        private INewsStore Store { get; }
        public NewsController(INewsStore store) => Store = store;


        [HttpGet]
        public async Task<IActionResult> List()
        {
            return View(await Store.ListAsync());
        }


        [HttpGet("{nid}/[action]")]
        public async Task<IActionResult> Delete(int nid)
        {
            var news = await Store.FindAsync(nid);
            if (news == null) return NotFound();

            return AskPost(
                title: $"Delete news {nid} - \"{news.Title}\"",
                message: $"You're about to delete news {nid} - \"{news.Title}\".\n" +
                    "Are you sure?",
                area: "Dashboard", controller: "News", action: "Delete",
                type: BootstrapColor.danger);
        }


        [HttpPost("{nid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int nid, int inajax)
        {
            var news = await Store.FindAsync(nid);
            if (news == null) return NotFound();

            await Store.DeleteAsync(news);
            StatusMessage = $"News {nid} deleted successfully.";
            await HttpContext.AuditAsync("deleted", $"{nid}");
            return RedirectToAction(nameof(List));
        }


        [HttpGet("{nid}/[action]")]
        public async Task<IActionResult> Edit(int nid)
        {
            var news = await Store.FindAsync(nid);
            if (news is null) return NotFound();

            return View(new NewsEditModel
            {
                MarkdownSource = Encoding.UTF8.GetString(news.Source),
                NewsId = news.Id,
                Active = news.Active,
                Title = news.Title,
            });
        }


        [HttpPost("{nid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int nid, NewsEditModel model,
            [FromServices] IMarkdownService markdownService)
        {
            if (model.NewsId != nid) return BadRequest();

            var news = await Store.FindAsync(nid);
            if (news is null) return NotFound();

            var document = markdownService.Parse(model.MarkdownSource);
            var html = markdownService.RenderAsHtml(document);
            var tree = markdownService.TocAsHtml(document);

            news.Source = Encoding.UTF8.GetBytes(model.MarkdownSource);
            news.Title = model.Title;
            news.Active = model.Active;
            news.Content = Encoding.UTF8.GetBytes(html);
            news.Tree = Encoding.UTF8.GetBytes(tree);
            news.LastUpdate = DateTimeOffset.Now;

            await Store.UpdateAsync(news);
            StatusMessage = "News updated successfully.";
            await HttpContext.AuditAsync("updated", $"{nid}");
            return RedirectToAction(nameof(Edit), new { nid });
        }


        [HttpGet("[action]")]
        public IActionResult Add()
        {
            return View("Edit", new NewsEditModel
            {
                MarkdownSource = "",
                NewsId = 0,
                Title = "标题",
                Active = false,
            });
        }


        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            NewsEditModel model,
            [FromServices] IMarkdownService markdownService)
        {
            var document = markdownService.Parse(model.MarkdownSource);
            var html = markdownService.RenderAsHtml(document);
            var tree = markdownService.TocAsHtml(document);

            var news = await Store.CreateAsync(new News
            {
                Source = Encoding.UTF8.GetBytes(model.MarkdownSource),
                Title = model.Title,
                Active = model.Active,
                LastUpdate = DateTimeOffset.Now,
                Content = Encoding.UTF8.GetBytes(html),
                Tree = Encoding.UTF8.GetBytes(tree),
            });

            StatusMessage = "News created successfully.";
            await HttpContext.AuditAsync("added", $"{news.Id}");
            return RedirectToAction("Edit", new { nid = news.Id });
        }
    }
}
