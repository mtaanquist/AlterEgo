using System;
using System.Threading.Tasks;
using AlterEgo.Data;
using AlterEgo.Models;
using AlterEgo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AlterEgo.Filters
{
    public class GuildAccessServiceFilter : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BattleNetApi _battleNetApi;

        public GuildAccessServiceFilter(ApplicationDbContext context, UserManager<ApplicationUser> userManager, BattleNetApi battleNetApi)
        {
            _context = context;
            _userManager = userManager;
            _battleNetApi = battleNetApi;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user =  await _userManager.GetUserAsync(context.HttpContext.User);

            if (user.LastApiQuery.AddDays(1) < DateTime.UtcNow)
            {
                // Time to query the API again, and update the users' characters.
                await _battleNetApi.GetUserCharactersAsync(user);
                await _battleNetApi.UpdateGuildRosterAsync();
                await _battleNetApi.UpdateGuildRanksAsync();
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
