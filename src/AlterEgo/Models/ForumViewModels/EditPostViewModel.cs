using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Models.ForumViewModels
{
    public sealed class EditPostViewModel
    {
        public Post Post { get; set; }

        public int PostId { get; set; }
        public string Content { get; set; }
    }
}
