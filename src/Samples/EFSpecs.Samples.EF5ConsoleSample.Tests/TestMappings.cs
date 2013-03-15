using NUnit.Framework;

namespace EFSpecs.Samples.EF5ConsoleSample.Tests
{
    [TestFixture]
    public class TestMappings
    {
        [Test]
        public void MappingShouldBeVerified()
        {
            new PersistenceSpecification<Contact>(() => new SampleContext())
                .CheckProperty(x => x.Name, "Kris McGinnes")
                .CheckProperty(x => x.Age, 29)
                .CheckReference(x => x.Address, new Address())
                .VerifyMappings();
        }
    }
}
