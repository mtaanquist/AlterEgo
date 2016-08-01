using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlterEgo.Data;
using AlterEgo.Helpers;
using AlterEgo.Models;
using AlterEgo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlterEgo.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BattleNetController : Controller
    {
        private const string GuildName = "Alter Ego";
        private const string Realm = "Argent Dawn";

        private readonly ApplicationDbContext _context;

        public BattleNetController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> UpdateNews()
        {
            var news = await BattleNetApi.GetGuildNews(Realm, GuildName);

            var currentNewsEntries = _context.News.ToList();
            news.RemoveAll(x => !currentNewsEntries.Contains(x));

            _context.News.AddRange(news);
            var result = await _context.SaveChangesAsync();

            return Json(new ApiStatusMessage { StatusCode = 200, Changes = result });
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> UpdateUserCharacters()
        {
            var users = _context.Users.ToList();
            var storedCharacters = _context.Characters.Include(c => c.User).AsNoTracking().ToList();

            var characters = new List<Character>();
            await users.ForEachAsync(async user =>
            {
                var userCharacters = await BattleNetApi.GetUserCharacters(user.AccessToken);
                userCharacters.ForEach(c => c.User = user);
                characters.AddRange(userCharacters);
            });

            // Add, update or delete characters in the stored list
            var newCharacters =
                characters.Where(c => !storedCharacters.Any(x => c.Name == x.Name && c.Realm == x.Realm))
                    .ToList();
            _context.AddRange(newCharacters);

            var removedCharacters =
                storedCharacters.Where(c => !characters.Any(x => c.Name == x.Name && c.Realm == x.Realm))
                    .ToList();
            removedCharacters.RemoveAll(c => c.User == null);
            _context.RemoveRange(removedCharacters);

            var changedCharacters =
                characters.Where(c => storedCharacters.Any(x => c.Name == x.Name && c.Realm == x.Realm))
                    .ToList();
            changedCharacters.RemoveAll(c => c.User == null);
            _context.UpdateRange(changedCharacters);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> UpdateGuildRoster()
        {
            var roster = await BattleNetApi.GetGuildRoster(Realm, GuildName);

            var characters = new List<Character>();
            roster.ForEach(member => characters.Add(member.Character));

            var storedCharacters = _context.Characters.Include(c => c.User).AsNoTracking().ToList();

            // Add, update or delete characters in the stored list
            var newCharacters =
                characters.Where(c => !storedCharacters.Any(x => c.Name == x.Name && c.Realm == x.Realm))
                    .ToList();
            _context.AddRange(newCharacters);

            var removedCharacters =
                storedCharacters.Where(c => !characters.Any(x => c.Name == x.Name && c.Realm == x.Realm))
                    .ToList();
            removedCharacters.RemoveAll(c => c.User != null);
            _context.RemoveRange(removedCharacters);

            var changedCharacters =
                characters.Where(c => storedCharacters.Any(x => c.Name == x.Name && c.Realm == x.Realm))
                    .ToList();
            changedCharacters.RemoveAll(c => c.User != null);
            _context.UpdateRange(changedCharacters);

            await _context.SaveChangesAsync();

            // Update the Member table
            var members = new List<Member>();
            await roster.ForEachAsync(async r => 
            {
                var member = new Member
                {
                    Character = await _context.Characters.SingleOrDefaultAsync(c => c.Name == r.Character.Name && c.Realm == r.Character.Realm),
                    CharacterName = r.Character.Name,
                    CharacterRealm = r.Character.Realm,
                    Rank = r.Rank
                };

                members.Add(member);
            });

            var storedMembers = await _context.Members.Include(m => m.Character).AsNoTracking().ToListAsync();

            // Add, update or delete members in the stored list
            var newMembers =
                members.Where(c => !storedMembers.Any(x => c.CharacterName == x.CharacterName && c.CharacterRealm == x.CharacterRealm))
                    .ToList();
            _context.AddRange(newMembers);

            var removedMembers =
                storedMembers.Where(c => !members.Any(x => c.CharacterName == x.CharacterName && c.CharacterRealm == x.CharacterRealm))
                    .ToList();
            _context.RemoveRange(removedMembers);

            var changedMembers =
                members.Where(c => storedMembers.Any(x => c.CharacterName == x.CharacterName && c.CharacterRealm == x.CharacterRealm))
                    .ToList();
            _context.UpdateRange(changedMembers);
            await _context.SaveChangesAsync();
            
            return Ok();
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> UpdateGuildRanks()
        {
            // Before running this, UpdateUsersCharacters and UpdateGuildRoster must have run first.

            var users = await _context.Users.Include(u => u.Characters).ToListAsync();
            users.ForEach(user => 
            {
                var currentRank = user.Rank;
                var characters = _context.Characters.Where(c => c.User == user && c.Realm == Realm && c.Guild == GuildName).ToList();
                characters.ForEach(character =>
                {
                    var member = _context.Members.SingleOrDefault(m => m.CharacterName == character.Name && m.CharacterRealm == character.Realm);
                    if (member != null)
                        currentRank = (member.Rank < currentRank) ? member.Rank : currentRank;
                });

                user.Rank = currentRank;
            });

            _context.UpdateRange(users);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}