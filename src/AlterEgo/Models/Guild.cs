using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlterEgo.Models
{
    public sealed class Guild
    {
        public string Name { get; set; }
        public string Realm { get; set; }

        public long LastModified { get; set; }
        
        public string Battlegroup { get; set; }
        public int Level { get; set; }
        public Faction Side { get; set; }
        public int AchievementPoints { get; set; }

        [NotMapped]
        public List<News> News { get; set; }
        public List<Member> Members { get; set; }
    }

    public sealed class News
    {
        public int NewsId { get; set; }

        // Foreign Keys
        public string GuildName { get; set; }
        public string GuildRealm { get; set; }
        public Guild Guild { get; set; }

        public string Type { get; set; }
        public string Character { get; set; }
        public Character GuildCharacter { get; set; }
        public long Timestamp { get; set; }
        public int ItemId { get; set; }
        public string Context { get; set; }
        public Achievement Achievement { get; set; }

        [NotMapped]
        public int[] BonusLists { get; set; }
        [NotMapped]
        public string FormattedBonusLists { get; set; }
    }

    public enum Faction
    {
        Alliance,
        Horde
    }
}