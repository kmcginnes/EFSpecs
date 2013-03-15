using System;
using System.Data.Entity;
using FluentAssertions;
using NUnit.Framework;

namespace EFSpecs.Samples.EF5ConsoleSample.Tests
{
    [TestFixture]
    public class TestMappings
    {
        [SetUp]
        public void Setup()
        {
            // TODO: Create script to new up database schema

            Database.SetInitializer<SampleContext>(null);

            using (var ctx = new SampleContext())
            {
                Console.WriteLine("Attempting to create database if it doesn't already exist");
                if (ctx.Database.CreateIfNotExists())
                {
                    Console.WriteLine("Database created");
                }
                else
                {
                    Console.WriteLine("Database already exists. Skipping creation");
                }
            }
        }

        [Test]
        public void MappingShouldBeVerified()
        {
            Action mappingTest = () =>
                                 new PersistenceSpecification<Contact>(() => new SampleContext())
                                     .CheckProperty(x => x.Name, "Kris McGinnes")
                                     .CheckProperty(x => x.Age, 29)
                                     .CheckReference(x => x.Address, new Address())
                                     .VerifyMappings();
            mappingTest.ShouldNotThrow<AssertionException>();
        }
    }
}
