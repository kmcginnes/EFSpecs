using System;
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
            new Database().Execute("ValidSchema.sql");
            System.Data.Entity.Database.SetInitializer<Context>(null);
        }

        [TearDown]
        public void TearDown()
        {
            new Database().Execute("TearDown.sql");
        }

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

        [Test]
        public void ValidateMappingRanTwiceShouldSucceed()
        {
            Action mappingTest = () =>
                                 new PersistenceSpecification<User>(() => new Context())
                                     .CheckReference(x => x.BillingAddress, new Address())
                                     .CheckProperty(x => x.Name, "Kris McGinnes")
                                     .CheckProperty(x => x.Age, 29)
                                     .VerifyMappings();
            mappingTest.ShouldNotThrow<AssertionException>();

            mappingTest = () =>
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

        [TearDown]
        public void TearDown()
        {
            new Database().Execute("TearDown.sql");
        }

        [Test]
        public void MappingShouldBeVerified()
        {
            Action mappingTest = () =>
                                 new PersistenceSpecification<User>(() => new Context())
                                     .CheckProperty(x => x.Name, "Kris McGinnes")
                                     .CheckProperty(x => x.Age, 29)
                                     .CheckReference(x => x.BillingAddress, new Address())
                                     .VerifyMappings();
            mappingTest.ShouldThrow<AssertionException>();
        }
    }
}
