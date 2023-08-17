using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecordStore.Data;
using RecordStore.ViewModels;

namespace RecordStore.Views.Shared.Components
{
    public class ArtistDropdownViewComponent : ViewComponent
    {
        private readonly RecordStoreContext _context;

        public ArtistDropdownViewComponent(RecordStoreContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? selectedArtistId = null)
        {
            var artists = await _context.Artist.ToListAsync();
            var viewModel = new ArtistDropdownViewModel
            {
                SelectedArtistId = selectedArtistId,
                Artists = artists
            };

            return View(viewModel);
        }
    }
}
