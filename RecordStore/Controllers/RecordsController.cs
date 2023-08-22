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
        private readonly IConfiguration _configuration;

        private string GenerateSAS(BlobClient blobClient, BlobContainerClient containerClient)
        {
            BlobSasBuilder sasBuilder = new()
            {
                BlobContainerName = containerClient.Name,
                BlobName = blobClient.Name,
                Resource = "b", // "b" indicates a blob
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            string sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential("recordstore", _configuration["AzureKey"])).ToString();

            return $"{blobClient.Uri}?{sasToken}";
        }

        public RecordsController(RecordStoreContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Records
        public async Task<IActionResult> Index()
        {

            if (_context.Record == null)
            {
                return Problem("Entity set 'RecordStoreContext.Record' is null.");
            }

            var records = await _context.Record.ToListAsync();

            BlobServiceClient serviceClient = new(_configuration["AzureConnectionString"]);
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient("recordimages");

            foreach (var record in records)
            {
                var artist = await _context.Artist.FindAsync(record.ArtistId);

                if (artist != null)
                {
                    ViewBag.ArtistName = ViewBag.ArtistName ?? new Dictionary<int, string>();
                    ViewBag.ArtistName[record.Id] = artist.Name;
                }

                BlobClient blobClient = containerClient.GetBlobClient(record.ImagePath);
                string sasUrl = GenerateSAS(blobClient, containerClient);
                record.ImagePath = sasUrl;
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
                BlobServiceClient serviceClient = new(_configuration["AzureConnectionString"]);
                BlobContainerClient containerClient = serviceClient.GetBlobContainerClient("recordimages");
                BlobClient blobClient = containerClient.GetBlobClient(record.Image.FileName);

                using (var stream = @record.Image.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

                record.ImagePath = record.Image.FileName;

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
