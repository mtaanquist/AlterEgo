using System.ComponentModel.DataAnnotations.Schema;

namespace AlterEgo.Models
{
    [NotMapped]
    public sealed class Member
    {
        public string CharacterName { get; set; }
        public string CharacterRealm { get; set; }
        public Character Character { get; set; }

        public int Rank { get; set; }
    }
}
