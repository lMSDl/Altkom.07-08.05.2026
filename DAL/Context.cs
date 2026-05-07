using Microsoft.EntityFrameworkCore;
using Models;

namespace DAL
{
    public class Context : DbContext
    {

        public Context() { }
        public Context(DbContextOptions<Context> options) : base(options) { }


        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            //włączamy tryb śledzenia zmian oparty na powiadomieniach o zmianach, co oznacza, że encje muszą implementować interfejs INotifyPropertyChanged i wywoływać metodę OnPropertyChanged w setterach właściwości, które chcemy śledzić
            modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified)
                .ToList()
                .ForEach(x => x.Property(nameof(Entity.CreatedDate)).IsModified = false);

            return base.SaveChanges();
        }
    }
}
