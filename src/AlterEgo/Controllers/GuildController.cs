using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AlterEgo.Models.GuildViewModels;
using AlterEgo.Services;
using AlterEgo.Data;
using AlterEgo.Models;

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
            var model = new RosterViewModel
            {
                Members = await BattleNetApi.GetGuildRoster("Argent Dawn", "Alter Ego")
            };

            var characters = new List<Character>();
            model.Members.ForEach(m =>
            {
                m.Character.CharacterRace = _context.Races.SingleOrDefault(r => r.Id == m.Character.Race);
                m.Character.CharacterClass = _context.Classes.SingleOrDefault(cl => cl.Id == m.Character.Class);
                characters.Add(m.Character);
            });

            await _battleNetDbHelper.UpdateStoredCharactersAsync(characters, null, "Alter Ego");

            return View(model);
        }
    }
}
