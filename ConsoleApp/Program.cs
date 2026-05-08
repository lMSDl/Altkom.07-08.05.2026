using ConsoleApp;
using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

var config = new DbContextOptionsBuilder<Context>()
    .UseSqlServer("Server=(local);Database=EF;TrustServerCertificate=True;Integrated Security=true");

using (var context = new Context(config.Options))
{
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}

//ChangeTracker.Run(config);
//ChangeTracker.RunProxies(config);
//ChangeTracker.RunNotifications(config);
//ConcurrencyCheck.Run(config);
//ShadowProperty.Run(config);
//GlobalFilters.Run(config);
//Transactions.Run(config);
//RelatedData.Run(config);
//TemporalTable.Run(config);
//CompileQuery.Run(config);
//BackingFields.Run(config);
//ComputedColumns.Run(config);


Converters.Run(config);

using (var context = new Context(config.Options))
{
    var products = context.Set<Product>().ToArray();
    context.Set<ProductDetails>().Load();

}