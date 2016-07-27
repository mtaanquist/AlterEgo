using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Models.ForumViewModels
{
    public class EditThreadViewModel
    {
        public Thread Thread { get; set; }
        
        public int ThreadId { get; set; }
        public int PostId { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }
    }
}
