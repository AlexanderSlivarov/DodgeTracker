namespace DodgeTracker.Models.Data
{
    public class LeagueEntry
    {
        public string? LeagueId { get; set; }
        public string? QueueType { get; set; }
        public string? Tier { get; set; }
        public string? Rank { get; set; }
        public int LeaguePoints { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}
