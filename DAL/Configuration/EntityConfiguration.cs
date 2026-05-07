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

            //konfiguracja globalnego filtra, który będzie stosowany do wszystkich zapytań dotyczących tej encji
            //builder.HasQueryFilter(e => !e.IsDeleted);

            //filtry możemy nazywać, co pozwala nam na stosowanie wielu filtrów do tej samej encji i wybieranie, który z nich chcemy zastosować w danym zapytaniu
            //jeśli nie podamy nazwy, to filtr będzie miał domyślną nazwę "Default", a jeśli podamy nazwę, to będzie ona używana do identyfikacji filtra
            //wiele filtrów o tej samej nazwie będzie się nadpisywać, więc warto stosować unikalne nazwy dla każdego filtra
            builder.HasQueryFilter("SoftDelete",e => !e.IsDeleted);
            //builder.HasQueryFilter("ModuloId",e => e.Id % 3 == 0);
        }
    }
}
