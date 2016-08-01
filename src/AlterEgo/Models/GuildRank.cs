using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AlterEgo.Models
{
    public enum GuildRank
    {
        [Display(Name = "Guild Master")]
        GuildMaster,
        Officer,
        [Display(Name = "Forum Admin")]
        ForumAdmin,
        Raider,
        Triallist,
        Social,
        [Display(Name = "Everyone (incl. non-guild members)")]
        Everyone = 99
    }
}
