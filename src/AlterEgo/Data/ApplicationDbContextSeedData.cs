using System.Linq;
using AlterEgo.Services;

namespace AlterEgo.Data
{
    public class ApplicationDbContextSeedData
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDbContextSeedData(ApplicationDbContext context)
        {
            _context = context;
        }

        public async void SeedData()
        {
            if (!_context.Guilds.Any(g => g.Name.Equals("Alter Ego")))
            {
                // Get key information on the guild
                var guild = await BattleNetApi.GetGuildProfile("Argent Dawn", "Alter Ego");
                _context.Guilds.Add(guild);
                await _context.SaveChangesAsync();
            }

            if (!_context.Classes.Any())
            {
                // Get the classes from the Battle.net API, then add them to the database.
                var classes = await BattleNetApi.GetClasses();
                _context.Classes.AddRange(classes);
                await _context.SaveChangesAsync();
            }

            if (!_context.Races.Any())
            {
                // Get the races from the Battle.net API, then add them to the database.
                var races = await BattleNetApi.GetRaces();
                _context.Races.AddRange(races);
                await _context.SaveChangesAsync();
            }
        }
    }
}
