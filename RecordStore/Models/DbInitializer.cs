using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RecordStore.Data;
using RecordStore.Data.Enums;

namespace RecordStore.Models
{
    public class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new RecordStoreContext(
                serviceProvider.GetRequiredService<DbContextOptions<RecordStoreContext>>()))
            {
                if (context.Record.Any() || context.Artist.Any())
                {
                    return; // DB has been seeded
                }

                Record timely = new()
                {
                    Title = "Timely!!",
                    ReleaseDate = new DateTime(1983, 12, 5),
                    Artist = "Anri",
                    Condition = Condition.Good
                };

                Record RideOnTime = new()
                {
                    Title = "Ride On Time",
                    ReleaseDate = new DateTime(1980, 9, 19),
                    Artist = "Tatsuro Yamashita",
                    Condition = Condition.VeryGoodPlus
                };

                List<Record> records = new()
                {
                    timely,
                    RideOnTime
                };

                context.Record.AddRange(records);

                var bioJSON = File.ReadAllText("Data/Bios/ArtistBios.json");
                var bios = JArray.Parse(bioJSON)[0];

                Artist anri = new()
                {
                    Name = "Anri",
                    Bio = bios["Anri"]["Bio"].ToString(),
                    Discography = new List<Record> { timely }
                };

                Artist tats = new()
                {
                    Name = "Tatsuro Yamashita",
                    Bio = bios["Tatsuro Yamashita"]["Bio"].ToString(),
                    Discography = new List<Record> { RideOnTime }
                };

                List<Artist> artists = new()
                {
                    anri,
                    tats
                };

                context.Artist.AddRange(artists);
                context.SaveChanges();
            }
        }
    }
}
