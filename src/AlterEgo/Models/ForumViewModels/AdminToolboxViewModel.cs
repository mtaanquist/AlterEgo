using System.Collections.Generic;

namespace AlterEgo.Models.ForumViewModels
{
    public sealed class AdminToolboxViewModel
    {
        // Meta
        public List<Category> Categories { get; set; }

        // New category
        public string CategoryName { get; set; }
        public int CategoryReadableBy { get; set; }

        // New forum
        public string ForumName { get; set; }
        public string ForumDescription { get; set; }
        public int CategoryId { get; set; }
        public int ForumReadableBy { get; set; } = 99;
        public int ForumWritableBy { get; set; } = 99;
        public int ForumCanLockThreads { get; set; } = 3;
        public int ForumCanStickyThreads { get; set; } = 3;
        public int ForumCanEditThreads { get; set; } = 3;
        public int ForumCanDeleteThreads { get; set; } = 3;
    }
}
