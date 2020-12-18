using Microsoft.AspNetCore.Mvc;
using SatelliteSite.NewsModule.Models;
using SatelliteSite.NewsModule.Services;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SatelliteSite.NewsModule.Controllers
{
    [Area("Tenant")]
    [Route("[controller]")]
    public class NewsController : ViewControllerBase
    {
        private INewsStore Store { get; }
        public NewsController(INewsStore adbc) => Store = adbc;


        [HttpGet("{nid}")]
        public async Task<IActionResult> Show(int nid)
        {
            var news = await Store.FindAsync(nid);
            var newsList = await Store.ListActiveAsync(100);

            if (news is null || !news.Active && !User.IsInRoles("Administrator"))
            {
                Response.StatusCode = 404;

                return View(new NewsViewModel
                {
                    NewsList = newsList,
                    NewsId = -1,
                    Title = "404 Not Found",
                    HtmlContent = "Sorry, the requested content is not found.",
                    LastUpdate = DateTimeOffset.Now,
                    Tree = "",
                });
            }
            else
            {
                return View(new NewsViewModel
                {
                    NewsList = newsList,
                    NewsId = nid,
                    Title = news.Title,
                    HtmlContent = Encoding.UTF8.GetString(news.Content),
                    LastUpdate = news.LastUpdate,
                    Tree = Encoding.UTF8.GetString(news.Tree),
                });
            }
        }
    }
}
