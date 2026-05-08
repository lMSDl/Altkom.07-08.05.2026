using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace DAL.Configuration
{
    internal class PersonConfiguration : EntityConfiguration<Person>
    {
        public override void Configure(EntityTypeBuilder<Person> builder)
        {
            base.Configure(builder);

            //konfiguracja tabeli temporalnej
            //builder.ToTable(x => x.IsTemporal());

            //konfiguracja tabeli temporalnej z niestandardową nazwą tabeli historii i kolumnami okresu
            builder.ToTable(x => x.IsTemporal(x =>
            {
                //wartości poniżej są domyślne, ale można je dostosować
                x.UseHistoryTable("PersonHistory");
                x.HasPeriodStart("PeriodStart");
                x.HasPeriodEnd("PeriodEnd");
            }));

            builder.Ignore(x => x.FirstNameLength);

            builder.Property(x => x.OptionalDescription).IsSparse();

            builder.OwnsOne(x => x.Address, navigationBuilder =>
            {
                navigationBuilder.ToJson();
                navigationBuilder.OwnsOne(x => x.Coordinates);
            });
        }
    }
}
