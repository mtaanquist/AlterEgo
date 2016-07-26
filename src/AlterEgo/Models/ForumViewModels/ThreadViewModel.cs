using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Models.ForumViewModels
{
    public class ThreadViewModel
    {
        public Thread Thread { get; set; }

        // Quick Reply
        public int ThreadId { get; set; }

        [Required]
        [MinLength(3)]
        public string Content { get; set; }
    }
}
