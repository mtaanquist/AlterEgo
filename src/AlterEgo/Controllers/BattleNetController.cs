using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlterEgo.Data;
using AlterEgo.Helpers;
using AlterEgo.Models;
using AlterEgo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AlterEgo.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BattleNetController : Controller
    {
        private const string GuildName = "Alter Ego";
        private const string Realm = "Argent Dawn";

        private readonly ApplicationDbContext _context;
        private readonly BattleNetApi _battleNetApi;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<BattleNetOptions> _options;

        public BattleNetController(ApplicationDbContext context, BattleNetApi battleNetApi, UserManager<ApplicationUser> userManager, IOptions<BattleNetOptions> options)
        {
            _context = context;
            _battleNetApi = battleNetApi;
            _userManager = userManager;
            _options = options;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ValidateAccessTokens()
        {
            var users = await _context.Users.ToListAsync();

            await users.ForEachAsync(async user =>
            {
                var hasValidToken = await BattleNetApi.CheckToken(user.AccessToken);
                if (!hasValidToken)
                {
                    user.AccessToken = string.Empty;
                    user.AccessTokenExpiry = DateTime.MinValue;
                    await _userManager.UpdateAsync(user);
                }
            });

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> UpdateNews()
        {
            var news = await _battleNetApi.GetGuildNews(Realm, GuildName);
            if (news == null)
                return BadRequest();

            var currentNewsEntries = _context.News.ToList();
            news.RemoveAll(x => !currentNewsEntries.Contains(x));

            _context.News.AddRange(news);
            var result = await _context.SaveChangesAsync();

            return Json(new ApiStatusMessage { StatusCode = 200, Changes = result });
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> UpdateSingleUserCharacters()
        {
            var user = await _userManager.FindByNameAsync("Karantor#2228");
            await _battleNetApi.GetUserCharactersAsync(user);

            return Ok();
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> UpdateUserCharacters()
        {
            await _battleNetApi.UpdateAllUserCharactersAsync();

            return Ok();
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> UpdateGuildRoster()
        {
            await _battleNetApi.UpdateGuildRosterAsync();

            return Ok();
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> UpdateGuildRanks()
        {
            await _battleNetApi.UpdateGuildRanksAsync();

            return Ok();
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> UpdateGuildAccess()
        {
            await _battleNetApi.UpdateAllUserCharactersAsync();
            await _battleNetApi.UpdateGuildRosterAsync();
            await _battleNetApi.UpdateGuildRanksAsync();

            return Ok();
        }
    }
}