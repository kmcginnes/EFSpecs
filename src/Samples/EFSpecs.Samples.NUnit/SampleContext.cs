using System.Data.Entity;

namespace EFSpecs.Samples.EF5ConsoleSample.Tests
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int BillingAddressId { get; set; }
        
        public Address BillingAddress { get; set; }
    }

    public class Address
    {
        public int AddressId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }

    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                        .HasRequired(a => a.BillingAddress)
                        .WithMany()
                        .HasForeignKey(u => u.BillingAddressId);
        }
    }
}