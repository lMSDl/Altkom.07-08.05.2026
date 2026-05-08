using DAL;
using Models;

namespace ConsoleApp
{
    internal class Views
    {
        public static void Run(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<DAL.Context> config)
        {
            Transactions.Run(config, false);
            using (var context = new Context(config.Options))
            {

                var summary = context.Set<OrderSummary>().ToList();
                foreach (var item in summary)
                {
                    Console.WriteLine($"Order ID: {item.Id}, Total Value: {item.TotalValue}, Order Date: {item.OrderDate}");
                }
            }
        }
    }
}
