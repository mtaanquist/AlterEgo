using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AlterEgo.Data;
using AlterEgo.Models;
using AlterEgo.Models.HomeViewModels;
using AlterEgo.Services;
using Microsoft.EntityFrameworkCore;

namespace AlterEgo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new IndexViewModel
            {
                News = await _context.News.OrderBy(x => x.Timestamp).ToListAsync(),
            };

            //return View(model);
            return RedirectToAction("Index", "Forum");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
