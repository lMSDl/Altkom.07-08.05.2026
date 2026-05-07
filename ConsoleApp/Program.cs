


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


ConcurrencyCheck.Run(config);



