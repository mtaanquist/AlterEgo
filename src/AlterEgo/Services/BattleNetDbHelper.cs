using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AlterEgo.Data;
using AlterEgo.Models;

namespace AlterEgo.Services
{
    public class BattleNetDbHelper
    {
        private readonly ApplicationDbContext _context;

        public BattleNetDbHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpdateStoredCharactersAsync(List<Character> characters, ApplicationUser user = null, string guild = null)
        {
            characters.ForEach(c =>
            {
                c.CharacterRace = _context.Races.SingleOrDefault(r => r.Id == c.Race);
                c.CharacterClass = _context.Classes.SingleOrDefault(cl => cl.Id == c.Class);
            });

            // Get stored characters, for comparisons
            var storedCharacters = await _context.Characters
                .Include(x => x.CharacterClass)
                .Include(x => x.CharacterRace)
                .ToListAsync();


            // Add, update or delete characters in the stored list
            var newCharacters =
                characters.Where(c => !storedCharacters.Any(x => x.Name.Equals(c.Name) && x.Realm.Equals(c.Realm)))
                    .ToList();

            var removedCharacters =
                storedCharacters.Where(c => !characters.Any(x => c.Name.Equals(x.Name) && x.Realm.Equals(c.Realm)))
                    .ToList();

            var changedCharacters =
                storedCharacters.Where(c => characters.Any(x => c.Name.Equals(x.Name) && x.Realm.Equals(c.Realm)))
                    .ToList();

            _context.AddRange(newCharacters);
            _context.RemoveRange(removedCharacters);
            _context.UpdateRange(changedCharacters);
            await _context.SaveChangesAsync();
        }
    }
}
