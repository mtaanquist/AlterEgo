using System.Collections.Generic;
using Sakura.AspNetCore;

namespace AlterEgo.Models.ForumViewModels
{
    public sealed class ThreadsViewModel
    {
        public List<Thread> Threads { get; set; }
        public IPagedList<Thread> PagedThreads { get; set; }

        public Forum Forum { get; set; }
    }
}
