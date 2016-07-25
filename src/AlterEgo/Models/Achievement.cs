using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlterEgo.Models
{
    public sealed class Achievement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Points { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public List<Criteria> Criteria { get; set; }
        public bool AccountWide { get; set; }
        public int FactionId { get; set; }
    }

    public sealed class Criteria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Description { get; set; }
        public int OrderIndex { get; set; }
        public int Max { get; set; }
    }
}