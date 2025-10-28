using DodgeTracker.Models;
using DodgeTracker.Models.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace DodgeTracker.Controllers
{
    public class TrackerController : Controller
    {
        private readonly HttpClient _httpClient;

        public TrackerController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost]
        public async Task<IActionResult> Index(string gameName, string tagLine)
        {
            string apiKey = "RGAPI-e9902679-6f1f-4e7d-b07b-e76ad63312f2";
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            string accountUrl = $"https://europe.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{gameName}/{tagLine}?api_key={apiKey}";
            HttpResponseMessage accountResponse = await _httpClient.GetAsync(accountUrl);

            if (!accountResponse.IsSuccessStatusCode)
            {
                ViewBag.Result = $"Error getting PUUID: {accountResponse.StatusCode}";
                return View();
            }

            string accountJson = await accountResponse.Content.ReadAsStringAsync();
            RiotAccount account = JsonSerializer.Deserialize<RiotAccount>(accountJson, jsonOptions);

            if (account == null || string.IsNullOrEmpty(account.Puuid))
            {
                ViewBag.Result = "Could not retrieve PUUID.";
                return View();
            }

            string accountPuuid = account.Puuid;

            string leagueUrl = $"https://eun1.api.riotgames.com/lol/league/v4/entries/by-puuid/{accountPuuid}?api_key={apiKey}";
            HttpResponseMessage leagueResponse = await _httpClient.GetAsync(leagueUrl);

            if (!leagueResponse.IsSuccessStatusCode)
            {
                ViewBag.Result = $"Error getting ranked info: {leagueResponse.StatusCode}";
                return View();
            }

            string leagueJson = await leagueResponse.Content.ReadAsStringAsync();
            List<LeagueEntry> leagues = JsonSerializer.Deserialize<List<LeagueEntry>>(leagueJson, jsonOptions);

            LeagueEntry soloQ = leagues?.FirstOrDefault(l => l.QueueType.Equals("RANKED_SOLO_5x5"));

            if (soloQ != null)
            {
                ViewBag.Result = $"{soloQ.Tier} {soloQ.Rank} - {soloQ.LeaguePoints} LP";                
            }
            else
            {
                ViewBag.Result = $"There is no data for player {account.GameName}#{account.TagLine} playing RANKED_SOLO_5x5 yet";                
            }

            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
