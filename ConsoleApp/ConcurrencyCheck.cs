using DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    internal class ConcurrencyCheck
    {
        public static void Run(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<Context> config)
        {
            using (var context = new Context(config.Options))
            {
                Order order = new Order { Name = "Test" };
                var product = new Product { Name = "Test", Price = 10, Order = order };
                order.Products.Add(product);

                context.Add(order);
                context.SaveChanges();

                ConcurrencyToken(context, order);

                RowVersion(context, product);

                ConflictResolve(config, order);
            }
        }


        private static void ConflictResolve(DbContextOptionsBuilder<Context> config, Order order)
        {
            config.LogTo(Console.WriteLine);
            using var context = new Context(config.Options);
            var product = new Product { Name = "Product 2", Price = 20, Order = order };
            context.Attach(order);
            context.Add(product);
            context.SaveChanges();

            product.Price = product.Price * 1.1m;

            ChangePrice(product.Id, config);

            bool saved = false;
            while (!saved)
            {
                try
                {
                    context.SaveChanges();
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        //wartości jakie chcemy wprowadzić do bazy danych
                        var currentValues = entry.CurrentValues;
                        //warttości jakie mamy w kontekście (jakie pobraliśmy z bazy danych)
                        var originalValues = entry.OriginalValues;
                        //wartości jakie są aktualnie w bazie danych
                        var databaseValues = entry.GetDatabaseValues();

                        switch (entry.Entity)
                        {
                            case Product p:

                                //var currentPrice = (decimal)currentValues[nameof(Product.Price)];
                                var currentPrice = currentValues.GetValue<decimal>(nameof(Product.Price));
                                var originalPrice = originalValues.GetValue<decimal>(nameof(Product.Price));
                                var databasePrice = databaseValues.GetValue<decimal>(nameof(Product.Price));

                                currentPrice = databasePrice + (currentPrice - originalPrice);

                                currentValues[nameof(Product.Price)] = currentPrice;

                                break;
                        }

                        //ustawiamy wartości z bazy danych jako oryginalne wartości, aby kolejne zapisy nie powodowały konfliktu
                        entry.OriginalValues.SetValues(databaseValues);
                    }

                }
            }

        }

        private static void ChangePrice(int id, DbContextOptionsBuilder<Context> config)
        {
            using var context = new Context(config.Options);
            var product = context.Set<Product>().Find(id);
            product.Price = product.Price + 10;
            context.SaveChanges();
        }

        static void ConcurrencyToken(Context context, Order order)
        {
            try
            {
                order.OrderDate = DateTime.Now;
                context.SaveChanges();

                order.Name = "Zamówienie 1";
                context.SaveChanges();
            }
            catch (Exception e)
            {

            }
        }

        static void RowVersion(Context context, Product product)
        {
            try
            {
                product.Price = 1;
                context.SaveChanges();

                product.Name = "Produkt 1";
                context.SaveChanges();
            }
            catch (Exception e)
            {

            }
        }

    }
}
