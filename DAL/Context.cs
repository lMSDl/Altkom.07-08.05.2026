using DAL.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Models;

namespace DAL
{
    public class Context : DbContext
    {

        public Context() { }
        public Context(DbContextOptions<Context> options) : base(options) { }



        public static Func<Context, int, Product> GetProductsByDateTime { get; } =
            EF.CompileQuery((Context context, int days) =>
                context.Set<Product>()
                    .Include(x => x.Order)
                    .ThenInclude(x => x.Products)
                    .Where(x => x.Id % 2 == 0)
                    .Where(x => x.Order.Id % 3 == 0)
                    .Where(x => x.Order.OrderDate < DateTime.Now.AddDays(days))
                    .OrderByDescending(x => x.Order.OrderDate)
                    .First());


        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(x => x.UseNetTopologySuite());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            //włączamy tryb śledzenia zmian oparty na powiadomieniach o zmianach, co oznacza, że encje muszą implementować interfejs INotifyPropertyChanged i wywoływać metodę OnPropertyChanged w setterach właściwości, które chcemy śledzić
            //modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            //możemy wpływać na sposób dostępu do właściwości w modelu, np. preferując dostęp przez właściwości zamiast pól
            modelBuilder.UsePropertyAccessMode(PropertyAccessMode.PreferProperty);
            //domyślne ustawienie to PropertyAccessMode.PreferFieldDuringConstruction, czyli dostęp przez pola podczas tworzenia obiektu, a potem przez właściwości
            //modelBuilder.UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);

            modelBuilder.Model.GetEntityTypes()
                .SelectMany(x => x.GetProperties())
                .Where(x => x.ClrType == typeof(int))
                .Where(x => x.Name == "Key")
                .ToList()
                .ForEach(x =>
                {
                    x.IsNullable = false;
                    ((IMutableEntityType)x.DeclaringType).SetPrimaryKey(x);
                });

            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            //configurationBuilder.Properties<DateTime>().HavePrecision(5);

            configurationBuilder.Conventions.Add(_ => new DateTimePrecisionConvention());
            configurationBuilder.Conventions.Add(_ => new PluralizeTableNameConvention());
            configurationBuilder.Conventions.Add(_ => new StringObfuscationConvention());

            //configurationBuilder.Conventions.Remove(typeof(KeyDiscoveryConvention));
        }


        public bool RandomFailure { get; set; }

        public override int SaveChanges()
        {
            if (RandomFailure && Random.Shared.Next(1, 25) == 1)
            {
                throw new Exception("Random failure");
            }

            /*ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified)
                .ToList()
                .ForEach(x => x.Property(nameof(Entity.CreatedDate)).IsModified = false);*/

            return base.SaveChanges();
        }
    }
}
