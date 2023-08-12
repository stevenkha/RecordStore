using System.ComponentModel.DataAnnotations;

namespace RecordStore.Models
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }
        public string? Image { get; set; }
        public required string Name { get; set; }
        public string? Bio { get; set; }
        public required List<Record> Discography { get; set; }
    }
}
