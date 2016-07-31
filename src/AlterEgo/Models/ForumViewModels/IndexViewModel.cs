using System.Collections.Generic;

namespace AlterEgo.Models.ForumViewModels
{
    public class IndexViewModel
    {
        public List<Category> Categories { get; set; }

        public Dictionary<int, Post> LatestPosts { get; set; }
    }
}
