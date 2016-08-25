using System;
using System.ComponentModel.DataAnnotations;

namespace AlterEgo.Models
{
    public class Character
    {
        public string Name { get; set; }
        public string Guild { get; set; }
        public string GuildRealm { get; set; }
        public GuildRank GuildRank { get; set; } = GuildRank.Everyone;

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

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public long LastModified { get; set; }


        public string MainCharacterUserId { get; set; }
        public ApplicationUser MainCharacterUser { get; set; }

        protected bool Equals(Character other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Realm, other.Realm);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Character) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (Realm != null ? Realm.GetHashCode() : 0);
            }
        }
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
