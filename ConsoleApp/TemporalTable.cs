using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ConsoleApp
{
    internal class TemporalTable
    {
        public static void Run(DbContextOptionsBuilder<Context> config)
        {
            using (var context = new Context(config.Options))
            {
                var person = new Person { FirstName = "John", LastName = "Doe" };
                context.Add(person);
                context.SaveChanges();

                Thread.Sleep(2500);

                person.FirstName = "Jane";
                context.SaveChanges();

                Thread.Sleep(2500);

                person.LastName = "Smith";
                context.SaveChanges();

                Thread.Sleep(2500);

                person.FirstName = "Jack";
                person.LastName = "Johnson";
                context.SaveChanges();

                context.ChangeTracker.Clear();

                person = context.Set<Person>().First();

                var data = context.Set<Person>().TemporalAll().Select(x => new { x, FROM = EF.Property<DateTime>(x, "PeriodStart"), TO = EF.Property<DateTime>(x, "PeriodEnd") }).ToArray();

                Console.WriteLine($"Obecny stan: {person.FirstName} {person.LastName}");

                person = context.Set<Person>().TemporalAsOf(DateTime.UtcNow.AddSeconds(-5)).First();

                Console.WriteLine($"Stan sprzed 5s: {person.FirstName} {person.LastName}");

                var history = context.Set<Person>().TemporalBetween(DateTime.UtcNow.AddSeconds(-10), DateTime.UtcNow.AddSeconds(-2)).ToArray();
            }
        }
    }
}
