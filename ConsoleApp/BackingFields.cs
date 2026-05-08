using DAL;
using Models;

namespace ConsoleApp
{
    internal class BackingFields
    {
        public static void Run(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<Context> config)
        {
            TemporalTable.Run(config);
            using (var context = new Context(config.Options))
            {
                var person = context.Set<Person>().First();
            }
        }
    }
}
