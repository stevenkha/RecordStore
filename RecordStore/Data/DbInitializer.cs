using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RecordStore.Models;

namespace RecordStore.Data
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
                context.Record.AddRange(
                    new Record
                    {
                        Title = "Ride On Time",
                        ReleaseDate = new DateTime(1980, 9, 19),
                        Artist = "Tatsuro Yamashita",
                        Condition = Enums.Condition.VeryGoodPlus
                    },
                    new Record 
                    {
                        Title = "Timely!!",
                        ReleaseDate = new DateTime(1983, 12, 5),
                        Artist = "Anri",
                        Condition = Enums.Condition.Good
                    }
                );

                var bioJSON = File.ReadAllText("Data/Bios/ArtistBios.json");
                var bios = JArray.Parse(bioJSON)[0];

                context.Artist.AddRange(
                    new Artist
                    {
                        Name = "Anri",
                        Bio = bios["Anri"]["Bio"].ToString(),
                        Discography = new List<Record>()
                    },
                    new Artist
                    {
                        Name = "Tatsuro Yamashita",
                        Bio = bios["Tatsuro Yamashita"]["Bio"].ToString(),
                        Discography = new List<Record>()
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
