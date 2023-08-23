using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecordStore.Data;
using RecordStore.Models;
using System.Globalization;

namespace RecordStore.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly RecordStoreContext _context;
        private readonly IConfiguration _configuration;

        public ArtistsController(RecordStoreContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

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

        // GET: Artists
        // TODO: caching for images
        public async Task<IActionResult> Index()
        {
            if (_context.Artist == null)
            {
                return Problem("Entity set 'RecordStoreContext.Artist' is null.");
            }

            var artists = await _context.Artist.Include(a => a.Discography).ToListAsync();
            
            foreach (Artist artist in artists)
            {
                // check for expired SAS urls
                Uri uri = new(artist.ImagePath);
                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                if (queryParams["se"] != null)
                {
                    string expiryTimeString = queryParams["se"];
                    if (DateTime.TryParseExact(expiryTimeString, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime expiryTime))
                    {
                        DateTime currentTime = DateTime.UtcNow;

                        if (expiryTime <= currentTime)
                        {
                            BlobServiceClient serviceClient = new BlobServiceClient(_configuration["AzureConnectionString"]);
                            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient("recordimages");
                            BlobClient blobClient = containerClient.GetBlobClient(artist.ImageName);

                            string newSasUrl = GenerateSAS(blobClient, containerClient);
                            artist.ImagePath = newSasUrl;

                            _context.Entry(artist).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }

            return View(artists);
        }

        // GET: Artists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Artist == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist
                .Include(a => a.Discography)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // GET: Artists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Image,Name,Bio")] Artist artist)
        {
            if (artist.Image != null)
            {
                long maxFileSize = 500 * 1024;

                if (artist.Image.Length > maxFileSize)
                {
                    ModelState.AddModelError("Image", "The uploaded image exceeds the maximum size 500KB");
                    return View(@artist);
                }
            }

            if (ModelState.IsValid)
            { 
                BlobServiceClient serviceClient = new(_configuration["AzureConnectionString"]);
                BlobContainerClient containerClient = serviceClient.GetBlobContainerClient("artistimages");

                string fileName = Guid.NewGuid().ToString();

                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                string sasURL = GenerateSAS(blobClient, containerClient);

                using (var stream = @artist.Image.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

                artist.ImagePath = sasURL;
                artist.ImageName = fileName;

                _context.Add(@artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        // GET: Artists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Artist == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Image,Name,Bio")] Artist artist)
        {
            if (id != artist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.Id))
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
            return View(artist);
        }

        // GET: Artists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Artist == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Artist == null)
            {
                return Problem("Entity set 'RecordStoreContext.Artist'  is null.");
            }
            var artist = await _context.Artist.FindAsync(id);
            if (artist != null)
            {
                _context.Artist.Remove(artist);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(int id)
        {
          return (_context.Artist?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
