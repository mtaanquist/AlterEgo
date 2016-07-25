using System.ComponentModel.DataAnnotations;

namespace AlterEgo.Models
{
    public class Character
    {
        public string Name { get; set; }
        public string Guild { get; set; }
        public string GuildRealm { get; set; }
        public int GuildRank { get; set; }

        public string Realm { get; set; }
        public string Battlegroup { get; set; }

        public int Class { get; set; }
        public Class CharacterClass { get; set; }
        public int Race { get; set; }
        public Race CharacterRace { get; set; }
        public int Gender { get; set; }

        public int Level { get; set; }
        public int AchievementPoints { get; set; }

        public string Thumbnail { get; set; }

        public int PlayerId { get; set; }
        public ApplicationUser Player { get; set; }
        public Member Member { get; set; }
        public long LastModified { get; set; }
    }

    public class Spec
    {
        [Key]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Role { get; set; }
        public int Order { get; set; }
        
        public string BackgroundImage { get; set; }
        public string Icon { get; set; }
    }
}
