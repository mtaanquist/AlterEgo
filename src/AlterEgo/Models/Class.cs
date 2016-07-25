using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlterEgo.Models
{
    public sealed class Class
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int Mask { get; set; }
        public string PowerType { get; set; }
        public string Name { get; set; }
    }
}
