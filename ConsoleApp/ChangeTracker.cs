using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ConsoleApp
{
    internal class ChangeTracker
    {
        public static void Run(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<Context> config)
        {
            using (var context = new Context(config.Options))
            {
                //domyślne ustawienie wykrywania zmian w kontekście ustawione jest na true
                context.ChangeTracker.AutoDetectChangesEnabled = false;


                Order order = new Order
                {
                    Name = "Zamówienie 1",
                    Products = new List<Product>
        {
            new Product { Name = "Produkt 1" },
            new Product { Name = "Produkt 2" },
            new Product { Name = "Produkt 3" }
        }
                };

                Console.WriteLine(context.ChangeTracker.DebugView.ShortView);

                Console.WriteLine("Order przed dodaniem do kontekstu: " + context.Entry(order).State);
                Console.WriteLine("Product przed dodaniem do kontekstu: " + context.Entry(order.Products.First()).State);
                //Detached - obiekt nie jest śledzony przez kontekst, nie jest on częścią kontekstu

                //context.Attach(order); //dołącza obiekt do kontekstu, ale w zależności od Id może ustawić stan na Unchanged lub Modified
                context.Add(order); //dołącza obiekt do kontekstu i ustawia stan na Added

                Console.WriteLine("Order po dodaniu do kontekstu: " + context.Entry(order).State);
                Console.WriteLine("Product po dodaniu do kontekstu: " + context.Entry(order.Products.First()).State);
                //Added - obiekt został dodany do kontekstu, ale zmiany nie zostały jeszcze zapisane do bazy danych


                Console.WriteLine(context.ChangeTracker.DebugView.ShortView);

                context.SaveChanges();

                Console.WriteLine("Order po zapisaniu do bazy: " + context.Entry(order).State);
                Console.WriteLine("Product po zapisaniu do bazy: " + context.Entry(order.Products.First()).State);
                //Unchanged - obiekt został zapisany do bazy danych, ale nie został zmieniony od ostatniego zapisu
                Console.WriteLine(context.ChangeTracker.DebugView.ShortView);


                order.OrderDate = DateTime.Now.AddDays(-1);
                var product1 = order.Products.First();
                var product2 = order.Products.Last();

                order.Products.Remove(product1);

                Console.WriteLine("Order po modyfikacji: " + context.Entry(order).State);
                Console.WriteLine("Order.Name po modyfikacji order: " + context.Entry(order).Property(o => o.Name).IsModified);
                Console.WriteLine("Order.OrderDate po modyfikacji order: " + context.Entry(order).Property(o => o.OrderDate).IsModified);
                Console.WriteLine("Order.Products po modyfikacji order: " + context.Entry(order).Collection(o => o.Products).IsModified);
                //Modified - obiekt został zmodyfikowany od ostatniego zapisu do bazy danych

                Console.WriteLine("Product 1: " + context.Entry(product1).State);
                Console.WriteLine("Product 2: " + context.Entry(product2).State);
                //Deleted - obiekt został oznaczony do usunięcia z bazy danych

                context.SaveChanges();

                Console.WriteLine("Product 1: " + context.Entry(product1).State);
                Console.WriteLine("Product 2: " + context.Entry(product2).State);

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);


                order.Products.Add(new Product { Name = "Produkt 4" });

                order = new Order
                {
                    Name = "Zamówienie 2",
                    Products = new List<Product>
        {
            new Product { Name = "Produkt 5" },
            new Product { Name = "Produkt 6" },
        }
                };

                context.Add(order);

                product2.Price = 100;

                //AutoDetectChanges działa w przypadku, gdy użyte zostanie SaveChanges, Entry, Local
                //w ostatnich zmianach nie używaliśmy żadnej z tych metod, więc zmiany nie zostały wykryte, a stan obiektów nie został zaktualizowany

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                context.ChangeTracker.DetectChanges();

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                context.SaveChanges();

                order.Name = "Zamówienie 2 - zmodyfikowane";
                order.OrderDate = DateTime.Now.AddDays(-2);

                context.ChangeTracker.DetectChanges();

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                //istnieje możliwość wyłączenia modyfikacji poszczególnych właściwości obiektu, np. Name, OrderDate, Products
                context.Entry(order).Property(x => x.OrderDate).IsModified = false;

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                context.SaveChanges();


                order.Name = "Zamówienie 2 - zmodyfikowane ponownie";
                order.CreatedDate = DateTime.Now.AddDays(-2);
                context.SaveChanges();

                context.ChangeTracker.Clear(); //odłącza wszystkie śledzone encje od kontekstu, zmienia ich stan na Detached, ale nie zmienia ich wartości

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
            }
        }

        public static void RunProxies(DbContextOptionsBuilder<Context> config)
        {
            //włączenie proxy dla śledzenia zmian, dzięki temu nie musimy ręcznie wywoływać DetectChanges, ponieważ proxy automatycznie wykrywa zmiany i aktualizuje stan obiektów
            config.UseChangeTrackingProxies();
            using (var context = new Context(config.Options))
            {
                context.ChangeTracker.AutoDetectChangesEnabled = false;

                /*var order = new Order()
                {
                    Name = "Zamówienie 3",
                    Products = new List<Product>
                    {
                        new Product { Name = "Produkt 8" },
                        new Product { Name = "Produkt 9" },
                        new Product { Name = "Produkt 10" }
                    }
                };*/
                //w przypadku używania proxy, nie możemy tworzyć obiektów za pomocą new, ponieważ nie będą one śledzone przez kontekst, musimy użyć metody CreateProxy, która tworzy obiekt proxy, który jest śledzony przez kontekst
                var order = context.CreateProxy<Order>();
                order.Name = "Zamówienie 3";
                order.Products.Add(context.CreateProxy<Product>());
                order.Products.Add(context.CreateProxy<Product>());
                order.Products.ElementAt(0).Name = "Produkt 8";
                order.Products.ElementAt(1).Name = "Produkt 9";

                context.Add(order);
                context.SaveChanges();
            }
        }

        public static void RunNotifications(DbContextOptionsBuilder<Context> config)
        {
            using (var context = new Context(config.Options))
            {
                context.ChangeTracker.AutoDetectChangesEnabled = false;

                var order = new Order { Name = "Zamówienie 4" };
                order.OrderDate = DateTime.Now.AddDays(-4);
                order.Products.Add(new Product { Name = "Produkt 11" });

                context.Add(order);

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                context.SaveChanges();

                order.OrderDate = DateTime.Now.AddDays(-3);

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                order.Name = "Zamówienie 4 - zmienione";

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                context.SaveChanges();

            }
        }
    }
}
