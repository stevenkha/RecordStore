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

                var bioJSON = File.ReadAllText("Data/Bios/ArtistBios.json");
                var bios = JArray.Parse(bioJSON)[0];

                Artist anri = new()
                {
                    Name = "Anri",
                    ImagePath = "https://recordstore.blob.core.windows.net/artistimages/104ec2dd-8d04-47f1-897d-f3ea29369239?sv=2023-01-03&st=2023-08-22T22%3A49%3A33Z&se=2023-08-22T23%3A49%3A33Z&sr=b&sp=r&sig=Sj0WAeYJRq%2FwWZcabkH0L%2FWh%2F442vo8fYKzRdCxewYg%3D",
                    Bio = bios["Anri"]["Bio"].ToString(),
                    Discography = new List<Record>()
                };

                Artist tats = new()
                {
                    Name = "Tatsuro Yamashita",
                    ImagePath = "https://recordstore.blob.core.windows.net/artistimages/5a0f7e95-8a17-4cf3-a59a-64f5f0b46ed7?sv=2023-01-03&st=2023-08-22T22%3A45%3A24Z&se=2023-08-22T23%3A45%3A24Z&sr=b&sp=r&sig=Z%2F6Ydmipw0NguEaFXLvcbFSQAHA%2FTBK0ptZ%2BPcLO0yo%3D",
                    Bio = bios["Tatsuro Yamashita"]["Bio"].ToString(),
                    Discography = new List<Record>()
                };

                List<Artist> artists = new()
                {
                    anri,
                    tats
                };

                context.Artist.AddRange(artists);

                Record timely = new()
                {
                    Title = "Timely!!",
                    ImagePath = "Timely!!.jpg",
                    ReleaseDate = new DateTime(1983, 12, 5),
                    ArtistId = anri.Id,
                    Condition = Condition.G
                };

                Record RideOnTime = new()
                {
                    Title = "Ride On Time",
                    ImagePath = "RideOnTime.jpg",
                    ReleaseDate = new DateTime(1980, 9, 19),
                    ArtistId = tats.Id,
                    Condition = Condition.VGPlus
                };

                anri.Discography.Add(timely);
                tats.Discography.Add(RideOnTime);

                List<Record> records = new()
                {
                    timely,
                    RideOnTime
                };

                context.Record.AddRange(records);

                context.SaveChanges();
            }
        }
    }
}
