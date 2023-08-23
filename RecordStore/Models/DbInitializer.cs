using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RecordStore.Data;
using RecordStore.Data.Enums;

namespace RecordStore.Models
{
    public class DbInitializer
    {
        private const string TimelyUrl = "https://recordstore.blob.core.windows.net/recordimages/d9f0c032-9892-49ea-99f6-da3034b10c80?sv=2023-01-03&st=2023-08-22T23%3A13%3A20Z&se=2023-08-23T00%3A13%3A20Z&sr=b&sp=r&sig=kyhWOz9KYBVl3JtoiMhOQWAh1ih1qL%2FgU4sqqpUJKyw%3D";
        private const string RideOnTimeUrl = "https://recordstore.blob.core.windows.net/recordimages/0d3af3a6-a512-4a6e-a26f-67b2aa5b695b?sv=2023-01-03&st=2023-08-22T23%3A12%3A18Z&se=2023-08-23T00%3A12%3A18Z&sr=b&sp=r&sig=S%2BGcZ1TlMwuQ%2Bsaw99C8xOuZzVNzQosC29WfgChAEwU%3D";

        private const string Anri = "https://recordstore.blob.core.windows.net/artistimages/104ec2dd-8d04-47f1-897d-f3ea29369239?sv=2023-01-03&st=2023-08-22T22%3A49%3A33Z&se=2023-08-22T23%3A49%3A33Z&sr=b&sp=r&sig=Sj0WAeYJRq%2FwWZcabkH0L%2FWh%2F442vo8fYKzRdCxewYg%3D";
        private const string Tatsuro = "https://recordstore.blob.core.windows.net/artistimages/5a0f7e95-8a17-4cf3-a59a-64f5f0b46ed7?sv=2023-01-03&st=2023-08-22T22%3A45%3A24Z&se=2023-08-22T23%3A45%3A24Z&sr=b&sp=r&sig=Z%2F6Ydmipw0NguEaFXLvcbFSQAHA%2FTBK0ptZ%2BPcLO0yo%3D";

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
                    ImagePath = Anri,
                    ImageName = "104ec2dd-8d04-47f1-897d-f3ea29369239",
                    Bio = bios["Anri"]["Bio"].ToString(),
                    Discography = new List<Record>()
                };

                Artist tats = new()
                {
                    Name = "Tatsuro Yamashita",
                    ImagePath = Tatsuro,
                    ImageName = "5a0f7e95-8a17-4cf3-a59a-64f5f0b46ed7",
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
                    ImagePath = TimelyUrl,
                    ImageName = "d9f0c032-9892-49ea-99f6-da3034b10c80",
                    ReleaseDate = new DateTime(1983, 12, 5),
                    ArtistId = anri.Id,
                    Condition = Condition.G
                };

                Record RideOnTime = new()
                {
                    Title = "Ride On Time",
                    ImagePath = RideOnTimeUrl,
                    ImageName = "0d3af3a6-a512-4a6e-a26f-67b2aa5b695b",
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
