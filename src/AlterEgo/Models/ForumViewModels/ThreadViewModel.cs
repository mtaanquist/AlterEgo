using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sakura.AspNetCore;

namespace AlterEgo.Models.ForumViewModels
{
    public class ThreadViewModel
    {
        public Thread Thread { get; set; }

        public List<Post> Posts { get; set; }
        public IPagedList<Post> PagedPosts { get; set; }

        // Quick Reply
        public int ThreadId { get; set; }

        [Required]
        [MinLength(3)]
        public string Content { get; set; }
    }
}
