using DAL;
using Models;

namespace ConsoleApp
{
    internal class ComputedColumns
    {
        //Computed Columns - kolumny obliczeniowe
        //kolumny, których wartość jest obliczana na podstawie innych kolumn w tabeli
        //wartość kolumny jest obliczana przez bazę danych podczas wstawiania lub aktualizacji rekordu
        //nie można bezpośrednio ustawić wartości kolumny obliczeniowej
        //przykład: kolumna TotalValue w tabeli Orders, która jest obliczana jako Value * (1 + Tax)
        public static void Run(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<DAL.Context> config)
        {
            using (var context = new Context(config.Options))
            {
                var order = new Order
                {
                    Name = "Test",
                    OrderDate = DateTime.Now,
                    Value = 100,
                    Tax = 0.23f
                };

                context.Add(order);
                context.SaveChanges();

                context.ChangeTracker.Clear();


                order = context.Set<Order>().First();

                Console.WriteLine($"Order: {order.Name}, Total Value: {order.TotalValue}");
            }
            Transactions.Run(config);
        }
    }
}
