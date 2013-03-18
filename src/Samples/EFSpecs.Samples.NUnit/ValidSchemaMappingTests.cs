using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace EFSpecs.Samples.EF5ConsoleSample.Tests
{
    [TestFixture]
    public class ValidSchemaMappingTests
    {
        [SetUp]
        public void Setup()
        {
            //new Database().Execute("ValidSchema.sql");
            //System.Data.Entity.Database.SetInitializer<Context>(null);

            // Uncomment this and comment the above to use EF to generate model.
            // EF generated models were work fine

            var user = new Context().Users.FirstOrDefault();
        }

        //[TearDown]
        //public void TearDown()
        //{
        //    new Database().Execute("TearDown.sql");
        //}

        [Test]
        public void MappingShouldBeVerified()
        {
            Action mappingTest = () =>
                                 new PersistenceSpecification<User>(() => new Context())
                                     .CheckReference(x => x.BillingAddress, new Address())
                                     .CheckProperty(x => x.Name, "Kris McGinnes")
                                     .CheckProperty(x => x.Age, 29)
                                     .VerifyMappings();
            mappingTest.ShouldNotThrow<AssertionException>();
        }
    }

    [TestFixture]
    public class MissingColumnInContactSchemaMappingTests
    {
        [SetUp]
        public void Setup()
        {
            new Database().Execute("MissingColumnInContactSchema.sql");
            System.Data.Entity.Database.SetInitializer<Context>(null);
        }

        [Test]
        public void MappingShouldBeVerified()
        {
            Action mappingTest = () =>
                                 new PersistenceSpecification<User>(() => new Context())
                                     .CheckProperty(x => x.UserId, 1)
                                     .CheckProperty(x => x.Name, "Kris McGinnes")
                                     .CheckProperty(x => x.Age, 29)
                                     //.CheckReference(x => x.Address, new Address())
                                     .VerifyMappings();
            mappingTest.ShouldNotThrow<AssertionException>();
        }
    }
}
