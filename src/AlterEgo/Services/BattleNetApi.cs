using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AlterEgo.Data;
using AlterEgo.Helpers;
using AlterEgo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AlterEgo.Services
{
    public class BattleNetApi
    {
        private const string Host = "https://eu.api.battle.net/";
        private const string Locale = "en_GB";

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<BattleNetOptions> _options;
        private readonly ILogger _logger;

        public BattleNetApi(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOptions<BattleNetOptions> options, ILoggerFactory loggerFactory)
        {
            _context = context;
            _userManager = userManager;
            _options = options;
            _logger = loggerFactory.CreateLogger<BattleNetApi>();
        }

        #region Web API Calls

        private async Task<string> Invoke(IDictionary<string, string> parameters)
        {
            var requestUrl =
                $"wow/{parameters["apiType"]}/{parameters["realm"]}/{parameters["name"]}?fields={parameters["fields"]}&locale={Locale}&apikey={_options.Value.ClientId}";
            string result = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Host);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                    result = await response.Content.ReadAsStringAsync();
            }

            return result;
        }

        public async Task<bool> IsAccessTokenValid(string accessToken)
        {
            var requestUrl = $"https://eu.battle.net/oauth/check_token?token={accessToken}";
            var result = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Host);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(requestUrl);

                result = (response.StatusCode != HttpStatusCode.BadRequest);
            }

            return result;
        }

        public async Task<List<Character>> GetUserCharacters(string accessToken)
        {
            var requestUrl =
                $"{Host}wow/user/characters" +
                    $"?client_id={_options.Value.ClientId}" +
                    $"&client_secret={_options.Value.ClientSecret}" +
                    $"&access_token={accessToken}";
            dynamic result = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Host);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    result = JObject.Parse(responseString).SelectToken("characters").ToObject<List<Character>>();
                }
            }

            return result;
        }

        public async Task<List<Race>> GetRaces()
        {
            var parameters = new Dictionary<string, string>
            {
                {"apiType", "data"},
                {"realm", "character"},
                {"name", "races"},
                {"fields", ""}
            };

            var response = await Invoke(parameters);

            if (!string.IsNullOrEmpty(response))
                return JObject.Parse(response).SelectToken("races").ToObject<List<Race>>();

            return null;
        }

        public async Task<List<Class>> GetClasses()
        {
            var parameters = new Dictionary<string, string>
            {
                {"apiType", "data"},
                {"realm", "character"},
                {"name", "classes"},
                {"fields", ""}
            };

            var response = await Invoke(parameters);

            if (!string.IsNullOrEmpty(response))
                return JObject.Parse(response).SelectToken("classes").ToObject<List<Class>>();

            return null;
        }

        public async Task<Guild> GetGuildProfile(string realm, string guildName, params string[] fields)
        {
            var fieldsString = string.Join(",", fields);
            var parameters = new Dictionary<string, string>
            {
                {"apiType", "guild"},
                {"realm", realm},
                {"name", guildName},
                {"fields", fieldsString}
            };

            var response = await Invoke(parameters);
            if (!string.IsNullOrEmpty(response))
                return JObject.Parse(response).ToObject<Guild>();

            return null;
        }

        public async Task<List<News>> GetGuildNews(string realm, string guildName)
        {
            var parameters = new Dictionary<string, string>
            {
                {"apiType", "guild"},
                {"realm", realm},
                {"name", guildName},
                {"fields", "news"}
            };

            var response = await Invoke(parameters);
            if (!string.IsNullOrEmpty(response))
                return JObject.Parse(response).SelectToken("news").ToObject<List<News>>();

            return null;
        }

        public async Task<List<Member>> GetGuildRoster(string realm, string guildName)
        {
            var parameters = new Dictionary<string, string>
            {
                {"apiType", "guild"},
                {"realm", realm},
                {"name", guildName},
                {"fields", "members"}
            };

            var response = await Invoke(parameters);
            if (!string.IsNullOrEmpty(response))
                return JObject.Parse(response).SelectToken("members").ToObject<List<Member>>();

            return null;
        }

        #endregion

        #region BattleNet Helpers

        public async Task UpdateUserCharactersAsync(ApplicationUser user)
        {
            var storedCharacters = _context.Characters.AsNoTracking().ToList();
            var characters = new List<Character>();

            if (!string.IsNullOrEmpty(user.AccessToken))
            {
                var userCharacters = await GetUserCharacters(user.AccessToken);

                if (userCharacters != null)
                {
                    userCharacters.ForEach(c =>
                    {
                        c.User = user;
                        c.UserId = user.Id;
                        c.CharacterRace = _context.Races.SingleOrDefault(r => r.Id == c.Race);
                        c.CharacterClass = _context.Classes.SingleOrDefault(cl => cl.Id == c.Class);
                    });
                    characters.AddRange(userCharacters);
                }
            }

            // Add, update or delete characters in the stored list
            var newCharacters =
                characters.Where(c => !storedCharacters.Any(x => c.Name == x.Name && c.Realm == x.Realm))
                    .ToList();
            _context.AddRange(newCharacters);

            var removedCharacters =
                storedCharacters.Where(c => !characters.Any(x => c.Name == x.Name && c.Realm == x.Realm))
                    .ToList();
            removedCharacters.RemoveAll(c => c.UserId == null);
            _context.RemoveRange(removedCharacters);

            var changedCharacters =
                characters.Where(c => storedCharacters.Any(x => c.Name == x.Name && c.Realm == x.Realm))
                    .ToList();
            changedCharacters.RemoveAll(c => c.UserId == null);
            _context.UpdateRange(changedCharacters);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAllUserCharactersAsync()
        {
            var users = _context.Users.ToList();
            var storedCharacters = _context.Characters.AsNoTracking().ToList();

            var characters = new List<Character>();
            await users.ForEachAsync(async user =>
            {
                if (!string.IsNullOrEmpty(user.AccessToken))
                {
                    var userCharacters = await GetUserCharacters(user.AccessToken);

                    if (userCharacters != null)
                    {
                        userCharacters.ForEach(c =>
                        {
                            c.User = user;
                            c.UserId = user.Id;
                            c.CharacterRace = _context.Races.SingleOrDefault(r => r.Id == c.Race);
                            c.CharacterClass = _context.Classes.SingleOrDefault(cl => cl.Id == c.Class);
                        });
                        characters.AddRange(userCharacters);
                    }
                }
            });

            // Add, update or delete characters in the stored list
            var newCharacters =
                characters.Where(c => !storedCharacters.Any(x => c.Name == x.Name && c.Realm == x.Realm))
                    .ToList();
            _context.AddRange(newCharacters);

            var removedCharacters =
                storedCharacters.Where(c => !characters.Any(x => c.Name == x.Name && c.Realm == x.Realm))
                    .ToList();
            removedCharacters.RemoveAll(c => c.UserId == null);
            _context.RemoveRange(removedCharacters);

            var changedCharacters =
                characters.Where(c => storedCharacters.Any(x => c.Name == x.Name && c.Realm == x.Realm))
                    .ToList();
            changedCharacters.RemoveAll(c => c.UserId == null);
            _context.UpdateRange(changedCharacters);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateGuildRosterAsync()
        {
            var roster = await GetGuildRoster(_options.Value.GuildRealm, _options.Value.GuildName);
            if (roster == null) return;

            var characters = new List<Character>();
            roster.ForEach(member => characters.Add(member.Character));

            characters.ForEach(c =>
            {
                c.CharacterRace = _context.Races.SingleOrDefault(r => r.Id == c.Race);
                c.CharacterClass = _context.Classes.SingleOrDefault(cl => cl.Id == c.Class);
            });

            var storedCharacters = _context.Characters.ToList();

            // Add, update or delete characters in the stored list
            var newCharacters =
                characters.Where(c => !storedCharacters.Any(x => c.Name == x.Name && c.Realm == x.Realm))
                    .ToList();
            _context.AddRange(newCharacters);

            var removedCharacters =
                storedCharacters.Where(c => !characters.Any(x => c.Name == x.Name && c.Realm == x.Realm))
                    .ToList();
            removedCharacters.RemoveAll(c => c.UserId != null);
            _context.RemoveRange(removedCharacters);

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
        }

        public async Task UpdateGuildRanksAsync()
        {
            // Before running this, UpdateUsersCharacters and UpdateGuildRoster must have run first.
            var users = await _context.Users.ToListAsync();
            users.ForEach(user =>
            {
                var currentRank = (int)GuildRank.Everyone;
                var characters = _context.Characters.Where(c => c.User == user && c.Realm == _options.Value.GuildRealm && c.Guild == _options.Value.GuildName).ToList();
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
        }

        #endregion
    }
}
