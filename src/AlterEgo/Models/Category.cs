using System.Collections.Generic;

namespace AlterEgo.Models
{
    public sealed class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public int SortOrder { get; set; }

        public int ReadableBy { get; set; }

        public bool IsDeleted { get; set; }
        
        public List<Forum> Forums { get; set; }
    }
}
