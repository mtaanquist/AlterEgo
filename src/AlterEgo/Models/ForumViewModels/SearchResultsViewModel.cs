using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakura.AspNetCore;

namespace AlterEgo.Models.ForumViewModels
{
    public class SearchResultsViewModel
    {
        public List<Thread> FoundThreads { get; set; }
        public IPagedList<Thread> PagedFoundThreads { get; set; }
    }
}
