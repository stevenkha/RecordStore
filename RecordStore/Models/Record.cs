using RecordStore.Data;
using System.ComponentModel.DataAnnotations;

namespace RecordStore.Models
{
    public class Record
    {
        [Key]
        public int Id { get; set; }
        public string? Image { get; set; } 
        public required string Title { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Release Date")]
        public required DateTime ReleaseDate { get; set; } 
        public required string Artist { get; set; }
        public required Condition Condition { get; set; }
    }
}
