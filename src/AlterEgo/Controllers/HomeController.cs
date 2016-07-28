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
        private readonly BattleNetDbHelper _battleNetDbHelper;

        public HomeController(ApplicationDbContext context, BattleNetDbHelper battleNetDbHelper)
        {
            _context = context;
            _battleNetDbHelper = battleNetDbHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View();
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
