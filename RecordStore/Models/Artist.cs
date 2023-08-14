using System.ComponentModel.DataAnnotations;

namespace RecordStore.Models
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }

        public string? Image { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string? Bio { get; set; }

        [Required]
        public List<Record> Discography { get; set; }

        public Artist() 
        {
            Discography = new List<Record>();
        }
    }
}
