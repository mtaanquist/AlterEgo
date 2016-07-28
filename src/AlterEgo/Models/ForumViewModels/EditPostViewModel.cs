using System.ComponentModel.DataAnnotations;

namespace AlterEgo.Models.ForumViewModels
{
    public sealed class EditPostViewModel
    {
        public Post Post { get; set; }

        public int PostId { get; set; }

        [MinLength(3)]
        public string Content { get; set; }
    }
}
