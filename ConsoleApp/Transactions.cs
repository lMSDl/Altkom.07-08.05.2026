using DAL;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ConsoleApp
{
    internal class Transactions
    {
        public static void Run(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<Context> config, bool randomFailure = true)
        {
            using (var context = new Context(config.Options))
            {
                context.RandomFailure = randomFailure;
                var products = Enumerable.Range(100, 50).Select(x => new Product { Name = $"Product {x}", Price = x * 10 }).ToArray();
                var orders = Enumerable.Range(1, 5).Select(x => new Order { OrderDate = DateTime.Now.AddDays(-x), Name = $"Order {x}" }).ToArray();

                //tworzymy transakcję
                //jeśli na kontekście została otwarta transakcja, to po wykonaniu operacji zapisu MUSI zostać wywołana metoda Commit - w przeciwny razie zmiany zostaną wycofane, a baza danych pozostanie w stanie sprzed rozpoczęcia transakcji
                using var transaction = context.Database.BeginTransaction();

                //tworząc transakcję, możemy określić poziom izolacji, który definiuje, jak transakcja będzie widziała zmiany
                //dostępne poziomy izolacji to:
                //Read Uncommitted - pozwala na odczyt danych, które zostały zmodyfikowane przez inne transakcje, ale jeszcze nie zostały zatwierdzone (tzw. brudne odczyty)
                //Read Committed - pozwala na odczyt danych, które zostały zatwierdzone przez inne transakcje, ale nie pozwala na odczyt danych, które zostały zmodyfikowane, ale jeszcze nie zostały zatwierdzone (tzw. niebrudne odczyty)
                //Repeatable Read - pozwala na odczyt danych, które zostały zatwierdzone przez inne transakcje, ale nie pozwala na odczyt danych, które zostały zmodyfikowane, ale jeszcze nie zostały zatwierdzone, oraz zapewnia, że dane odczytane przez transakcję nie mogą być zmodyfikowane przez inne transakcje do momentu zakończenia transakcji (tzw. powtarzalne odczyty)
                //Serializable - zapewnia, że transakcja jest całkowicie izolowana od innych transakcji, co oznacza, że dane odczytane przez transakcję nie mogą być zmodyfikowane przez inne transakcje do momentu zakończenia transakcji, a także zapewnia, że dane odczytane przez transakcję są spójne z danymi odczytanymi przez inne transakcje (tzw. serializowalne odczyty)
                //Snapshot - pozwala na odczyt danych, które zostały zatwierdzone przez inne transakcje, ale nie pozwala na odczyt danych, które zostały zmodyfikowane, ale jeszcze nie zostały zatwierdzone, oraz zapewnia, że dane odczytane przez transakcję są spójne z danymi odczytanymi przez inne transakcje w momencie rozpoczęcia transakcji (tzw. migawkowe odczyty)
                //Unspecified - pozwala na użycie domyślnego poziomu izolacji ustawionego w bazie danych, który jest zwykle Read Committed
                //Chaos - pozwala na odczyt danych, które zostały zmodyfikowane przez inne transakcje, ale jeszcze nie zostały zatwierdzone, oraz pozwala na odczyt danych, które zostały zmodyfikowane, ale jeszcze nie zostały zatwierdzone, oraz pozwala na odczyt danych, które zostały zmodyfikowane przez inne transakcje, ale jeszcze nie zostały zatwierdzone (tzw. chaotyczne odczyty)
                //using var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

                try
                {
                    for (int i = 0; i < orders.Length; i++)
                    {
                        var savePoint = $"SavePoint_{i}";
                        transaction.CreateSavepoint(savePoint);
                        try
                        {
                            var subproducts = products.Skip(i * 10).Take(10).ToArray();
                            foreach (var product in subproducts)
                            {
                                context.Add(product);
                                context.SaveChanges();
                            }

                            var order = orders[i];
                            order.Products = subproducts;
                            context.Add(order);
                            context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
                            transaction.RollbackToSavepoint(savePoint); //cofamy zmiany dokonane od momentu utworzenia savepointa
                        }

                        if (context.RandomFailure && Random.Shared.Next(1, 10) == 1)
                        {
                            throw new Exception("BIG random failure");
                        }

                        //czyścimy ChangeTracker, aby uniknąć zapisywania danych, któe zostały wycofane przez RollbackToSavepoint, ponieważ są one nadal śledzone przez ChangeTracker i mogą zostać zapisane przy wywołaniu SaveChanges
                        context.ChangeTracker.Clear();
                    }

                    //zatwierdzamy transakcję, co powoduje trwałe zapisanie wszystkich zmian w bazie danych
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    //jeśli wystąpił błąd, wycofujemy transakcję, co powoduje anulowanie wszystkich zmian dokonanych w bazie danych od momentu rozpoczęcia transakcji
                    transaction.Rollback();
                }
            }

        }
        private void CrossContextTransaction()
        {

            using var connection = new SqlConnection("Server=(local);Database=EF;TrustServerCertificate=True;Integrated Security=true");

            using var connectionTransation = connection.BeginTransaction();

            var options = new DbContextOptionsBuilder<Context>()
                .UseSqlServer(connection)
                .Options;


            using var context1 = new Context(options);
            using var context2 = new Context(options);

            context1.Database.UseTransaction(connectionTransation);
            context2.Database.UseTransaction(connectionTransation);
        }
    }

}
