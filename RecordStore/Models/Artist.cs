using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecordStore.Models
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string? Bio { get; set; }

        [Required]
        public List<Record> Discography { get; set; } = new List<Record>();
    }
}
