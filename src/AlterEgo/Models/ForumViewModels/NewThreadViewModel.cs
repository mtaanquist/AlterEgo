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
        public string ThreadSubject { get; set; }
        [Required]
        [MinLength(3)]
        [DisplayName("Content")]
        public string PostContent { get; set; }

        public bool StickyThread { get; set; }
        public bool LockThread { get; set; }
    }
}
