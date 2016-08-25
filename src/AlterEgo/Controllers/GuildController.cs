using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AlterEgo.Models.GuildViewModels;
using AlterEgo.Data;
using AlterEgo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AlterEgo.Controllers
{
    public class GuildController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOptions<BattleNetOptions> _options;

        public GuildController(ApplicationDbContext context, IOptions<BattleNetOptions> options)
        {
            _context = context;
            _options = options;
        }

        public async Task<IActionResult> Roster()
        {
            var characters =
                await
                    _context.Characters
                        .AsNoTracking()
                        .Include(c => c.CharacterRace)
                        .Include(c => c.CharacterClass)
                        .Include(c => c.User)
                        .Where(
                            character =>
                                character.Guild == _options.Value.GuildName &&
                                character.Realm == _options.Value.GuildRealm &&
                                character.GuildRank != GuildRank.Everyone)
                        .OrderBy(character => character.GuildRank)
                        .ThenBy(character => character.Name)
                        .ToListAsync();

            var model = new RosterViewModel
            {
                Characters = characters
            };

            return View(model);
        }
    }
}
