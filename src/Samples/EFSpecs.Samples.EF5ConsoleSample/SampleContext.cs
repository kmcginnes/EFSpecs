using System.Data.Entity;

namespace EFSpecs.Samples.EF5ConsoleSample
{
    public class SampleContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }
    }
}