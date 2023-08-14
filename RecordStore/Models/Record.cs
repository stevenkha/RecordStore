using RecordStore.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace RecordStore.Models
{
    public class Record
    {
        [Key]
        public int Id { get; set; }

        public string? Image { get; set; }

        [Required]
        public string Title { get; set; } = "";

        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public Artist Artist { get; set; }

        [Required]
        public Condition Condition { get; set; }

        public Record()
        {
            Artist = new Artist()
            {
                Name = "",
                Discography = new List<Record>()
            };
        }
    }
}
