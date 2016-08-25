using System;
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
        public DateTime AccessTokenExpiry { get; set; }
        public int FailedTokenValidations { get; set; }

        public int Rank { get; set; }

        // User Options
        public string LocalTimeZoneInfoId { get; set; } = "Europe/Paris";
        public string MainCharacterName { get; set; }
        public string MainCharacterRealm { get; set; }
        [InverseProperty("MainCharacterUser")]
        public Character MainCharacter { get; set; }

        // Last Activity
        public DateTime RegisteredAt { get; set; }
        public DateTime LastActivity { get; set; }
        public List<ThreadActivity> ThreadActivities { get; set; }

        // API Update Details
        public DateTime LastApiQuery { get; set; }

        // Permission helpers, not mapped
        [NotMapped]
        public bool IsModerator => (Rank <= (int)GuildRank.ForumAdmin);

        [InverseProperty("User")]
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

    public sealed class ThreadActivity
    {
        public int ThreadActivityId { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int ThreadId { get; set; }
        public int ForumId { get; set; }
        public DateTime LastRead { get; set; }
        public int LastReadPostId { get; set; }
    }
}
