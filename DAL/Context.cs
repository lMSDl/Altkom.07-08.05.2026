using Microsoft.EntityFrameworkCore;

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

            base.OnModelCreating(modelBuilder);
        }
    }
}
