using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AlterEgo.Data;
using AlterEgo.Helpers;
using AlterEgo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        public BattleNetApi(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IOptions<BattleNetOptions> options, ILoggerFactory loggerFactory)
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
            var result = "";

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

        private async Task<List<Character>> GetUserCharactersAsync(string accessToken)
        {
            const string apiUrl = "wow/user/characters";
            List<Character> result = null;

            using (var client = new HttpClient())
            {
                var credentials = Encoding.ASCII.GetBytes($"{_options.Value.ClientId}:{_options.Value.ClientSecret}");
                var values = new Dictionary<string, string> { { "access_token", accessToken } };
                var content = new FormUrlEncodedContent(values);

                var requestUri = new Uri($"{Host}{apiUrl}");

                client.BaseAddress = new Uri(Host);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(credentials));

                var response = await client.PostAsync(requestUri, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    result = JObject.Parse(responseString).SelectToken("characters").ToObject<List<Character>>();

                    _logger.LogInformation(100, $"Retrieved {result.Count} characters for access token '{accessToken}'");
                }
                else
                {
                    _logger.LogError(100,
                        $"Failed to retrieve characters for access token '{accessToken}' - Result: {response.StatusCode}");
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

        private async Task<List<Member>> GetGuildRosterAsync(string realm, string guildName)
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

        public async Task GetUserCharactersAsync(ApplicationUser user)
        {
            // First, we check if the user's token is still valid.
            var isUserAccessTokenValid = !string.IsNullOrEmpty(user.AccessToken) &&
                                         user.AccessTokenExpiry > DateTime.UtcNow &&
                                         await CheckToken(user.AccessToken);

            if (!isUserAccessTokenValid)
            {
                // Token isn't valid, so remove it from the database, which will log the user out
                user.AccessToken = "";
                user.AccessTokenExpiry = DateTime.MinValue;
                await _userManager.UpdateAsync(user);

                _logger.LogWarning((int)BattleNetEvents.GetUserCharacters,
                    $"Could not retrieve characters for user {user.UserName} as their access token is no longer valid.");
                return;
            }

            var races = await _context.Races.ToListAsync();
            var classes = await _context.Classes.ToListAsync();
            var userCharacters = await GetUserCharactersAsync(user.AccessToken);

            if (userCharacters == null || !userCharacters.Any())
            {
                _logger.LogError((int)BattleNetEvents.GetUserCharacters,
                    $"An error occurred attempting to retrieve {user.UserName}'s characters.");
                return;
            }

            userCharacters.ForEach(character =>
            {
                character.UserId = user.Id;
                character.User = user;

                character.CharacterClass = classes.Single(c => c.Id == character.Class);
                character.CharacterRace = races.Single(race => race.Id == character.Race);

                if (_context.Characters.Contains(character))
                {
                    _context.Characters.Update(character);
                }
                else
                {
                    character.GuildRank = GuildRank.Everyone;
                    _context.Characters.Add(character);
                }
            });

            var storedUserCharacters =
                await _context.Characters.Where(character => character.UserId == user.Id).ToListAsync();

            storedUserCharacters.ForEach(character =>
            {
                if (!userCharacters.Contains(character))
                {
                    _context.Characters.Remove(character);
                }
            });

            user.LastApiQuery = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAllUserCharactersAsync()
        {
            var users = _context.Users.ToList();
            await users.ForEachAsync(async user =>
            {
                await GetUserCharactersAsync(user);
            });

        }

        public async Task UpdateGuildRosterAsync()
        {
            var races = await _context.Races.ToListAsync();
            var classes = await _context.Classes.ToListAsync();

            var guildCharacters = new List<Character>();
            var members = await GetGuildRosterAsync(_options.Value.GuildRealm, _options.Value.GuildName);

            if (members == null || !members.Any())
            {
                _logger.LogError((int)BattleNetEvents.GetUserCharacters,
                    $"An error occurred attempting to retrieve the guild roster.");
                return;
            }

            members.ForEach(member =>
            {
                guildCharacters.Add(member.Character);

                if (!_context.Characters.Contains(member.Character))
                {
                    var character = member.Character;
                    character.CharacterClass = classes.Single(c => c.Id == character.Class);
                    character.CharacterRace = races.Single(race => race.Id == character.Race);
                    character.GuildRank = (GuildRank)member.Rank;

                    _context.Characters.Add(member.Character);
                }
                else
                {
                    var character = _context.Characters.Single(c => c.Equals(member.Character));
                    character.GuildRank = (GuildRank)member.Rank;

                    _context.Characters.Update(character);
                }
            });

            var storedCharacters =
                await _context.Characters.Where(character => string.IsNullOrEmpty(character.UserId)).ToListAsync();
            storedCharacters.ForEach(character =>
            {
                if (!guildCharacters.Contains(character))
                {
                    _context.Characters.Remove(character);
                }
            });

            await _context.SaveChangesAsync();
        }

        public async Task UpdateGuildRanksAsync()
        {
            // Before running this, UpdateUsersCharacters and UpdateGuildRoster must have run first.
            var users = await _context.Users.ToListAsync();
            users.ForEach(user =>
            {
                var newRank = (int)GuildRank.Everyone;
                var characters =
                    _context.Characters.Where(
                        character =>
                            character.UserId == user.Id && character.Realm == _options.Value.GuildRealm &&
                            character.Guild == _options.Value.GuildName).ToList();

                if (characters != null && characters.Any())
                {
                    newRank = (int)characters.Min(x => x.GuildRank);
                }

                user.Rank = newRank;
            });

            _context.Users.UpdateRange(users);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Private Functions

        public static async Task<bool> CheckToken(string accessToken)
        {
            var requestUrl = $"https://eu.battle.net/oauth/check_token?token={accessToken}";
            bool result;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Host);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var obj = JObject.Parse(responseString);

                    result = (obj.Property("scope") != null);
                }
                else
                {
                    result = (response.StatusCode == HttpStatusCode.BadRequest);
                }
            }

            return result;
        }

        #endregion
    }

    public enum BattleNetEvents
    {
        GetUserCharacters = 100,
        GetGuildNews = 101,
        GetGuildRoster = 102,
        UpdateUserRanks = 103
    }
}
