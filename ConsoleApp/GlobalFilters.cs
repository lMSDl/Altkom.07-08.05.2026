using DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    internal class GlobalFilters
    {

        public static void Run(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<Context> config)
        {
            ShadowProperty.Run(config);

            using (var context = new Context(config.Options))
            {
                var products = context.Set<Models.Product>().Where(x => x.Id % 2 == 0).ToArray();
                foreach (var product in products)
                {
                    product.IsDeleted = true;
                }
                context.SaveChanges();
                context.ChangeTracker.Clear();


                //products = context.Set<Models.Product>().Where(x => !x.IsDeleted).ToArray();
                products = context.Set<Models.Product>().ToArray();
                foreach (var product in products)
                {
                    Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, IsDeleted: {product.IsDeleted}");
                }

                //var orderProducts = context.Set<Order>().Where(x => !x.IsDeleted).SelectMany(x => x.Products).Where(x => !x.IsDeleted).ToArray();
                var orderProducts = context.Set<Order>().SelectMany(x => x.Products).ToArray();
                foreach (var product in orderProducts)
                {
                    Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, IsDeleted: {product.IsDeleted}");
                }

                //ignorujemy globalne filtry i pobieramy wszystkie produkty
                //możemy też podać nazwy filtrów, który chcemy zignorować, jeśli mamy zdefiniowanych kilka globalnych filtrów
                var allProducts = context.Set<Product>().IgnoreQueryFilters(/*["SoftDelete"]*/).ToArray();
                foreach (var product in allProducts)
                {
                    Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, IsDeleted: {product.IsDeleted}");
                }
            }
        }
    }
}
