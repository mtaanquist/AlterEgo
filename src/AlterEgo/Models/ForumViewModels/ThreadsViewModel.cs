using System.Collections.Generic;

namespace AlterEgo.Models.ForumViewModels
{
    public sealed class ThreadsViewModel
    {
        public List<Thread> Threads { get; set; }

        public Forum Forum { get; set; }
    }
}
