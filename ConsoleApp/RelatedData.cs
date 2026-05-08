using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ConsoleApp
{
    internal class RelatedData
    {

        public static void Run(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<Context> config)
        {
            Transactions.Run(config, false);
            config.LogTo(Console.WriteLine);

            using (var context = new Context(config.Options))
            {
                //Eager loading - ładowanie danych z główym obiektem

                Console.Clear();
                //var products = context.Set<Product>().Include(x => x.Order).ToArray();
                //AsNoTracking - wyłączenie śledzenia zmian dla pobieranych obiektów, co może poprawić wydajność przy dużej liczbie danych, ale uniemożliwia późniejsze aktualizacje tych obiektów
                //dane po pobraniu z AsNoTracking nie są załączane do kontekstu oraz nie są wiązane ze sobą (ordery o tym samym id jest innym obiektem w pamięci)
                var products = context.Set<Product>()/*.AsNoTracking()*/.Include(x => x.Order).Where(x => x.Id % 2 == 0).ToArray();
                products = context.Set<Product>().Include(x => x.Order).ThenInclude(x => x.Products).Where(x => x.Id % 2 == 0).ToArray();

                context.ChangeTracker.Clear();

                //AsSplitQuery - rozdzielenie zapytania na kilka mniejszych, aby uniknąć problemu N+1
                products = context.Set<Product>().AsSplitQuery().Include(x => x.Order).ThenInclude(x => x.Products).Where(x => x.Id % 2 == 0).ToArray();
            }

            using (var context = new Context(config.Options))
            {
                //Explicit loading - ładowanie danych na żądanie
                var product = context.Set<Product>().First();

                context.Entry(product).Reference(x => x.Order).Load();

                if (product.Order is not null)
                    context.Entry(product.Order).Collection(x => x.Products).Load();

                context.ChangeTracker.Clear();

                context.Set<Product>().Load();
                context.Set<Order>().Load();
                Console.Clear();
                var products = context.Set<Product>().Local.Where(x => x.Id % 3 == 0).ToArray();
            }

            //Lazy loading - ładowanie danych w momencie odwołania się do nich
            Product lazyProduct;
            using (var context = new Context(config.Options))
            {
                lazyProduct = context.Set<Product>().First();

                Console.WriteLine(lazyProduct.Order?.Name);

                context.ChangeTracker.Clear();

                lazyProduct = context.Set<Product>().First();
            }

            Console.WriteLine(lazyProduct.Order?.Name);


            Order order;
            config.UseLazyLoadingProxies(); //włączenie lazy loadingu na podstawie proxy
            using (var context = new Context(config.Options))
            {
                order = context.Set<Order>().First();

                Console.Clear();
                var products = order.Products;

                context.ChangeTracker.Clear();
                order = context.Set<Order>().First();
            }

            var orderProducts = order.Products;
            Console.WriteLine();
        }
    }
}
