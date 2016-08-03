using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AlterEgo.Data;
using AlterEgo.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace AlterEgo.Services
{
    public class BattleNetApi
    {
        private const string Host = "https://eu.api.battle.net/";
        private const string Locale = "en_GB";

        private readonly IOptions<BattleNetOptions> _options;

        public BattleNetApi(IOptions<BattleNetOptions> options)
        {
            _options = options;
        }

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
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
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

            var result = JObject.Parse(response).SelectToken("races").ToObject<List<Race>>();
            return result;
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

            var result = JObject.Parse(response).SelectToken("classes").ToObject<List<Class>>();
            return result;
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
            var result = JObject.Parse(response).ToObject<Guild>();

            return result;
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
            var result = JObject.Parse(response).SelectToken("news").ToObject<List<News>>();

            return result;
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
            var result = JObject.Parse(response).SelectToken("members").ToObject<List<Member>>();

            return result;
        }
    }
}
