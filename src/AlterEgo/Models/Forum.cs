using System.Collections.Generic;

namespace AlterEgo.Models
{
    public sealed class Forum
    {
        public int ForumId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ReadableBy { get; set; }
        public int WritableBy { get; set; }
        public int CanLockThreads { get; set; }
        public int CanStickyThreads { get; set; }
        public int CanEditThreads { get; set; }
        public int CanDeleteThreads { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<Thread> Threads { get; set; }

        public Post LatestPost { get; set; }
    }
}
