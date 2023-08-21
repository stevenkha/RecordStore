using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecordStore.Data;
using RecordStore.Models;

namespace RecordStore.Controllers
{
    public class RecordsController : Controller
    {
        private readonly RecordStoreContext _context;
        private readonly BlobServiceClient _serviceClient;

        public RecordsController(RecordStoreContext context, BlobServiceClient serviceClient)
        {
            _context = context;
            _serviceClient = serviceClient;
        }

        // GET: Records
        public async Task<IActionResult> Index()
        {

            if (_context.Record == null)
            {
                return Problem("Entity set 'RecordStoreContext.Record' is null.");
            }

            var records = await _context.Record.ToListAsync();

            foreach (var record in records)
            {
                var artist = await _context.Artist.FindAsync(record.ArtistId);

                if (artist != null)
                {
                    ViewBag.ArtistName = ViewBag.ArtistName ?? new Dictionary<int, string>();
                    ViewBag.ArtistName[record.Id] = artist.Name;
                }
            }

            return View(records);
        }

        // GET: Records/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Record == null)
            {
                return NotFound();
            }

            var @record = await _context.Record.FirstOrDefaultAsync(m => m.Id == id);
            if (@record == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist.FindAsync(record.ArtistId);
            if (artist == null)
            {
                return NotFound();
            }

            ViewBag.ArtistName = artist.Name;
            return View(@record);
        }

        // GET: Records/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Records/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Image,Title,ReleaseDate,ArtistId,Condition")] Record @record)
        {
            if (ModelState.IsValid)
            {
                BlobContainerClient containerClient = _serviceClient.GetBlobContainerClient("recordimages");

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(@record.Image.FileName);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                BlobSasBuilder sasBuilder = new()
                {
                    BlobContainerName = containerClient.Name,
                    BlobName = blobClient.Name,
                    Resource = "b", // "b" indicates a blob
                    StartsOn = DateTimeOffset.UtcNow,
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read); // Set the desired permissions

                string sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential("recordstore", "eWAn91JgyRBNrts1zz6qqIz+Kn7E+njjRnUxzRag86RBnIj/Je8eRiEHYnhckd1EOKUirvMjYfet+ASt6Pbh7g==")).ToString();

                // Construct SAS URL
                string sasUrl = $"{blobClient.Uri}?{sasToken}";
                record.ImagePath = sasUrl;

                using (var stream = @record.Image.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

                _context.Add(@record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@record);
        }

        // GET: Records/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Record == null)
            {
                return NotFound();
            }

            // TODO: make artist a dropdown and display available artists
            var @record = await _context.Record.FindAsync(id);
            if (@record == null)
            {
                return NotFound();
            }

            return View(@record);
        }

        // POST: Records/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Image,Title,ReleaseDate,ArtistId,Condition")] Record @record)
        {
            // TODO: Validate Artist input
            if (id != @record.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@record);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecordExists(@record.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@record);
        }

        // GET: Records/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Record == null)
            {
                return NotFound();
            }

            var @record = await _context.Record.FirstOrDefaultAsync(m => m.Id == id);
            if (@record == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist.FindAsync(record.ArtistId);
            if (artist == null)
            {
                return NotFound(nameof(Artist));
            }

            ViewBag.ArtistName = artist.Name;

            return View(@record);
        }

        // POST: Records/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Record == null)
            {
                return Problem("Entity set 'RecordStoreContext.Record'  is null.");
            }
            var @record = await _context.Record.FindAsync(id);
            if (@record != null)
            {
                _context.Record.Remove(@record);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecordExists(int id)
        {
          return (_context.Record?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
