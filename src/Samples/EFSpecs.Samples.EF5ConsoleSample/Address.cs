using System.ComponentModel.DataAnnotations.Schema;

namespace EFSpecs.Samples.EF5ConsoleSample
{
    public class Address
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }
    }
}