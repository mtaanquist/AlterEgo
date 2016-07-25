using System;
using System.Collections.Generic;

namespace AlterEgo.Models
{
    public sealed class Thread
    {
        public int ThreadId { get; set; }
        public string Name { get; set; }
        public bool IsLocked { get; set; }
        public bool IsStickied { get; set; }
        public bool IsDeleted { get; set; }

        public string AuthorUserId { get; set; }
        public ApplicationUser Author { get; set; }
        public DateTime CreatedAt { get; set; }

        public int EditorUserId { get; set; }
        public ApplicationUser Editor { get; set; }
        public DateTime ModifiedAt { get; set; }

        public List<Post> Posts { get; set; }

        public int ForumId { get; set; }
        public Forum Forum { get; set; }
    }
}
