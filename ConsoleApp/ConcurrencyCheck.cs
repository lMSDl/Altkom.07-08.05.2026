using DAL;
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
            }
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
