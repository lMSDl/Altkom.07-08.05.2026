using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ConsoleApp
{
    internal class Converters
    {
        public static void Run(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<Context> config)
        {
            Transactions.Run(config, false);
            using (var context = new Context(config.Options))
            {
                var orders = context.Set<Order>().Include(o => o.Products).ToArray();
                foreach (var order in orders)
                {
                    Console.WriteLine($"Order: {order.Name}, Type: {order.Type}, Parameters: {string.Join(", ", Enum.GetNames<OrderParameters>().Where(p => order.Parameters.HasFlag(Enum.Parse<OrderParameters>(p))))}, Products: {string.Join(", ", order.Products.Select(p => p.Name))}");
                }
            }
        }
    }
}
