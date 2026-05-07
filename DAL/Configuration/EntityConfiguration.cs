using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace DAL.Configuration
{
    internal abstract class EntityConfiguration<T> : IEntityTypeConfiguration<T> where T : Entity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            //tworzenie shadow property
            //uzywamy wersji generycznej funkcji Property podając typ kolumny i nazwę kolumny + konfiguracja
            //shadow property nie jest dostępne w modelu, ale jest dostępne w kontekście
            builder.Property<DateTime>("CreatedDate").HasDefaultValueSql("GETDATE()");
        }
    }
}
