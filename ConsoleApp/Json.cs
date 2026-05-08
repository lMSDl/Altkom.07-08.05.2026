using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    internal class Json
    {
        public static void Run(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<DAL.Context> config)
        {
            config.LogTo(Console.WriteLine);
            using var context = new Context(config.Options);

            var person = new Person
            {
                FirstName = "John",
                LastName = "Doe",
                Address = new Address
                {
                    Street = "123 Main St",
                    City = "Anytown",
                    PostalCode = "12345",
                    Coordinates = new Coordinates
                    {
                        Latitude = 40.7128f,
                        Longitude = -74.0060f
                    }
                }
            };
            context.Add(person);

            person = new Person
            {
                FirstName = "Jane",
                LastName = "Smith",
                Address = new Address
                {
                    Street = "456 Elm St",
                    City = "Othertown",
                    PostalCode = "67890",
                    Coordinates = new Coordinates
                    {
                        Latitude = 34.0522f,
                        Longitude = -118.2437f
                    }
                }
            };
            context.Add(person);

            context.SaveChanges();

            context.ChangeTracker.Clear();

            person = context.Set<Person>().Where(x => x.Address.City == "Othertown").First();

            person.Address.PostalCode = "30-001";
            person.Address.Coordinates.Latitude = 51;
            context.SaveChanges();
        }
    }
}
