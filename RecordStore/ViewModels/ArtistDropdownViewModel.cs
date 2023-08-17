using RecordStore.Models;

namespace RecordStore.ViewModels
{
    public class ArtistDropdownViewModel
    {
        public int? SelectedArtistId { get; set; }
        public required List<Artist> Artists { get; set; }
    }
}
