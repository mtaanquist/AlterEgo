using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Models.ForumViewModels
{
    public class NewThreadViewModel
    {
        public int ForumId { get; set; }
        public Forum Forum { get; set; }

        public string ThreadSubject { get; set; }
        public string PostContent { get; set; }
    }
}
