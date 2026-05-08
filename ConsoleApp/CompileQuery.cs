using DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Diagnostics;

namespace ConsoleApp
{
    internal class CompileQuery
    {

        public static void Run(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<Context> config)
        {
            Transactions.Run(config, false);

            config.LogTo(Console.WriteLine);


            using (var context = new Context(config.Options))
            {
                context.Set<Product>().Load();
            }

            Stopwatch timer1;
            using (var context = new Context(config.Options))
            {
                Console.Clear();
                timer1 = Stopwatch.StartNew();
                var prodcut = context.Set<Product>()
                    .Include(x => x.Order)
                    .ThenInclude(x => x.Products)
                    .Where(x => x.Id % 2 == 0)
                    .Where(x => x.Order.Id % 3 == 0)
                    .Where(x => x.Order.OrderDate < DateTime.Now.AddDays(5))
                    .OrderByDescending(x => x.Order.OrderDate)
                    .First();
                timer1.Stop();
            }

            Stopwatch timer2;
            using (var context = new Context(config.Options))
            {
                Console.Clear();
                timer2 = Stopwatch.StartNew();
                var prodcut = context.Set<Product>()
                    .Include(x => x.Order)
                    .ThenInclude(x => x.Products)
                    .Where(x => x.Id % 2 == 0)
                    .Where(x => x.Order.Id % 3 == 0)
                    .Where(x => x.Order.OrderDate < DateTime.Now.AddDays(10))
                    .OrderByDescending(x => x.Order.OrderDate)
                    .First();
                timer2.Stop();
            }


            Stopwatch timer3;
            using (var context = new Context(config.Options))
            {
                Console.Clear();
                timer3 = Stopwatch.StartNew();
                var prodcut = context.Set<Product>()
                    .Include(x => x.Order)
                    .ThenInclude(x => x.Products)
                    .Where(x => x.Id % 2 == 0)
                    .Where(x => x.Order.Id % 3 == 0)
                    .Where(x => x.Order.OrderDate < DateTime.Now.AddDays(15))
                    .OrderByDescending(x => x.Order.OrderDate)
                    .First();
                timer3.Stop();
            }

            Stopwatch timer4;
            using (var context = new Context(config.Options))
            {
                Console.Clear();
                timer4 = Stopwatch.StartNew();
                var prodcut = Context.GetProductsByDateTime(context, 4);
                timer4.Stop();
            }

            Stopwatch timer5;
            using (var context = new Context(config.Options))
            {
                Console.Clear();
                timer5 = Stopwatch.StartNew();
                var prodcut = Context.GetProductsByDateTime(context, 9);
                timer5.Stop();
            }


            Stopwatch timer6;
            using (var context = new Context(config.Options))
            {
                Console.Clear();
                timer6 = Stopwatch.StartNew();
                var prodcut = Context.GetProductsByDateTime(context, 14);
                timer6.Stop();
            }


            Console.WriteLine($"Czas wykonywania zwykłego zapytania 1: {timer1.ElapsedMilliseconds} ms");
            Console.WriteLine($"Czas wykonywania zwykłego zapytania 2: {timer2.ElapsedMilliseconds} ms");
            Console.WriteLine($"Czas wykonywania zwykłego zapytania 3: {timer3.ElapsedMilliseconds} ms");
            Console.WriteLine($"Czas wykonywania skompilowanego zapytania 1: {timer4.ElapsedMilliseconds} ms");
            Console.WriteLine($"Czas wykonywania skompilowanego zapytania 2: {timer5.ElapsedMilliseconds} ms");
            Console.WriteLine($"Czas wykonywania skompilowanego zapytania 3: {timer6.ElapsedMilliseconds} ms");
        }
    }
}
