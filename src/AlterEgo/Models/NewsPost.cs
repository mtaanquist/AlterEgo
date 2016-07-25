using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Models
{
    public sealed class NewsPost
    {
        public int PostId { get; set; }
     
        public string Title { get; set; }
        public string Content { get; set; }

        [NotMapped]
        public string FormattedContent { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        public ApplicationUser Author { get; set; }
        public PostCategory Category { get; set; }
    }

    public sealed class PostCategory
    {
        public int PostCategoryId { get; set; }

        public string Name { get; set; }

        public List<NewsPost> Posts { get; set; }
    }
}
