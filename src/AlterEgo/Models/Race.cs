using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlterEgo.Models
{
    public sealed class Race
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int Mask { get; set; }
        public string Side { get; set; }
        public string Name { get; set; }
    }
}
