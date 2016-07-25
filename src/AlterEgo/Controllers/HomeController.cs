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
            var model = new IndexViewModel
            {
                News = await BattleNetApi.GetGuildNews("Argent Dawn", "Alter Ego")
            };

            model.News.ForEach(x => {
                x.FormattedBonusLists = string.Join(":", x.BonusLists);
                x.GuildCharacter = _context.Characters
                    .Include(c => c.CharacterClass)
                    .Include(c => c.CharacterRace)
                    .SingleOrDefault(c => c.Name.Equals(x.Character) && c.Realm.Equals("Argent Dawn"));
            });

            var roster = await BattleNetApi.GetGuildRoster("Argent Dawn", "Alter Ego");
            var characters = roster.Select(member => member.Character).ToList();
            await _battleNetDbHelper.UpdateStoredCharactersAsync(characters);

            return View(model);
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
