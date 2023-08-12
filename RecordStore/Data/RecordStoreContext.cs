using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecordStore.Models;

namespace RecordStore.Data
{
    public class RecordStoreContext : DbContext
    {
        public RecordStoreContext (DbContextOptions<RecordStoreContext> options)
            : base(options)
        {
        }

        public DbSet<Artist> Artist { get; set; } = default!;
    }
}
