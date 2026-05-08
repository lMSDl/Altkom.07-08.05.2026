using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ConsoleApp
{
    internal class SplitTable
    {
        public static void Run(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<Context> config)
        {
            Converters.Run(config);

            using (var context = new Context(config.Options))
            {
                var products = context.Set<Product>().ToArray();
                context.Set<ProductDetails>().Load();
            }
        }
    }
}
