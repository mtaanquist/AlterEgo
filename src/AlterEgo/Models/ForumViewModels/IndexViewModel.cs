using System.Collections.Generic;

namespace AlterEgo.Models.ForumViewModels
{
    public class IndexViewModel
    {
        public List<Category> Categories { get; set; }

        public Dictionary<int, Post> LatestPosts { get; set; }

        public int TotalThreads { get; set; }
        public int TotalPosts { get; set; }
        public int TotalMembers { get; set; }

        public List<ApplicationUser> ActiveUsers { get; set; }
    }
}
