namespace AlterEgo.Models
{
    public sealed class Member
    {
        public string GuildName { get; set; }
        public string GuildRealm { get; set; }
        public Guild Guild { get; set; }
        public string CharacterName { get; set; }
        public string CharacterRealm { get; set; }
        public Character Character { get; set; }
        public int Rank { get; set; }
    }
}
