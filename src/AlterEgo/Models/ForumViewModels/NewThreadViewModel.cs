using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AlterEgo.Models.ForumViewModels
{
    public class NewThreadViewModel
    {
        public int ForumId { get; set; }
        public Forum Forum { get; set; }

        [Required]
        [MinLength(3)]
        [DisplayName("Subject")]
        public string Subject { get; set; }
        [Required]
        [MinLength(3)]
        [DisplayName("Content")]
        public string Content { get; set; }

        public bool StickyThread { get; set; }
        public bool LockThread { get; set; }
    }
}
