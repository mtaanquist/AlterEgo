using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AlterEgo.Models.GuildViewModels;
using AlterEgo.Services;
using AlterEgo.Data;
using AlterEgo.Models;
using Microsoft.EntityFrameworkCore;

namespace AlterEgo.Controllers
{
    public class GuildController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BattleNetDbHelper _battleNetDbHelper;

        public GuildController(ApplicationDbContext context, BattleNetDbHelper battleNetDbHelper)
        {
            _context = context;
            _battleNetDbHelper = battleNetDbHelper;
        }

        public async Task<IActionResult> Roster()
        {
            var roster = await _context.Members
                .Include(member => member.Character)
                .ThenInclude(character => character.CharacterClass)
                .Include(member => member.Character)
                .ThenInclude(character => character.CharacterRace)
                .ToListAsync();

            var model = new RosterViewModel
            {
                Members = roster
            };

            return View(model);
        }
    }
}
