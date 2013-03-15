using System.ComponentModel.DataAnnotations.Schema;

namespace EFSpecs.Samples.EF5ConsoleSample.Tests
{
    public class Address
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }
    }
}