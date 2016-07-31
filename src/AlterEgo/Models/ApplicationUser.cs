using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AlterEgo.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        // Access Token stuff
        public string AccessToken { get; set; }
        public string AccessTokenExpiry { get; set;}

        public int GuildRank { get; set; }

        // Permission helpers, not mapped
        [NotMapped]
        public bool IsModerator => (GuildRank < 4);

        public List<Character> Characters { get; set; }
        
        [InverseProperty("Author")]
        public List<Thread> AuthoredThreads { get; set; }

        [InverseProperty("Editor")]
        public List<Thread> EditedThreads { get; set; }

        [InverseProperty("Author")]
        public List<Post> AuthoredPosts { get; set; }

        [InverseProperty("Editor")]
        public List<Post> EditedPosts { get; set; }
    }
}
