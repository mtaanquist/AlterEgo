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
            var storedCharacters = _context.Characters.AsNoTracking().ToList();

            var characters = new List<Character>();
            await users.ForEachAsync(async user =>
            {
                var userCharacters = await BattleNetApi.GetUserCharacters(user.AccessToken);
                userCharacters.ForEach(c => c.Player = user);
                characters.AddRange(userCharacters);
            });

            // Add, update or delete characters in the stored list
            var removedCharacters =
                storedCharacters.Where(c => !characters.Any(x => c.Name.Equals(x.Name) && x.Realm.Equals(c.Realm)))
                    .ToList();
            _context.RemoveRange(removedCharacters);
            await _context.SaveChangesAsync();

            var changedCharacters =
                characters.Where(c => storedCharacters.Any(x => c.Name.Equals(x.Name) && x.Realm.Equals(c.Realm)))
                    .ToList();
            _context.UpdateRange(changedCharacters);
            await _context.SaveChangesAsync();

            var newCharacters =
                characters.Where(c => !storedCharacters.Any(x => x.Name.Equals(c.Name) && x.Realm.Equals(c.Realm)))
                    .ToList();
            _context.AddRange(newCharacters);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}