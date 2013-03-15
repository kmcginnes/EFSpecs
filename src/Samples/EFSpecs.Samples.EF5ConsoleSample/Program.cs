using System;
using System.Data.Entity;

namespace EFSpecs.Samples.EF5ConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
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

            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
        }
    }
}
