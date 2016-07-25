using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Models
{
    public sealed class Post
    {
        public int PostId { get; set; }
        public string Content { get; set; }

        public string AuthorUserId { get; set; }
        public ApplicationUser Author { get; set; }
        public DateTime PostedAt { get; set; }

        public int EditorUserId { get; set; }
        public ApplicationUser Editor { get; set; }
        public DateTime EditedAt { get; set; }

        public int ThreadId { get; set; }
        public Thread Thread { get; set; }
    }
}
