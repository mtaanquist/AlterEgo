using System.ComponentModel.DataAnnotations;

namespace AlterEgo.Models.ForumViewModels
{
    public sealed class NewReplyViewModel
    {
        public Thread Thread { get; set; }

        public int ThreadId { get; set; }

        [Required]
        [MinLength(3)]
        public string Content { get; set; }
    }
}
