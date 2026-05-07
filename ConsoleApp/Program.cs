


using ConsoleApp;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Models;

var config = new DbContextOptionsBuilder<Context>()
    .UseSqlServer("Server=(local);Database=EF;TrustServerCertificate=True;Integrated Security=true");

using (var context = new Context(config.Options))
{
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}

//ChangeTracker.Run(config);
//ChangeTracker.RunProxies(config);
//ChangeTracker.RunNotifications(config);
//ConcurrencyCheck.Run(config);


config.LogTo(Console.WriteLine);
using(var context = new Context(config.Options))
{
    for(int i = 1; i <= 13; i++)
    {
        var order = new Order { Name = $"Zamówienie {i}" };
        var orderProduct = new Product { Name = $"Produkt {i}", Price = i * 10, Order = order };
        context.Add(orderProduct);
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);
    }
    context.SaveChanges();
    context.ChangeTracker.Clear();




    var product = context.Set<Product>().Skip(Random.Shared.Next(12)).First();

    //nie możemy pobrać Id dla orderu, ponieważ Order jest null - nie jest on załadowany do kontekstu
    //var orderId = product.Order.Id;

    //pobieramy wartość shadow property OrderId, która jest dostępna w kontekście, mimo że nie jest ona zdefiniowana w modelu
    var orderId = context.Entry(product).Property<int>("OrderId").OriginalValue;
    Console.WriteLine(orderId);

    var createdAt = context.Entry(product).Property<DateTime>("CreatedDate").OriginalValue;
    Console.WriteLine(createdAt);

    //modyfikacja wartości shadow property OrderId
    context.Entry(product).Property<int>("OrderId").CurrentValue = 1;

    Console.WriteLine(context.ChangeTracker.DebugView.LongView);

    context.SaveChanges();

    //wyszukujemy produkty z OrderId = 1 używając funkcji EF.Property, która pozwala na dostęp do wartości shadow property w zapytaniu LINQ
    var products = context.Set<Product>().Where(x => EF.Property<int>(x, "OrderId") == 1).ToArray();
}
