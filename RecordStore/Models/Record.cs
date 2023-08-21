using RecordStore.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecordStore.Models
{
    public class Record
    {
        [Key]
        public int Id { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        public string Title { get; set; } = "";

        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        [Required]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Artist")]
        [Required]
        public int ArtistId { get; set; }

        [Required]
        public Condition Condition { get; set; }
    }
}
