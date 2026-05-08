using DAL.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Models;

namespace DAL.Configuration
{
    internal class OrderConfiguration : EntityConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name).IsConcurrencyToken();

            //Computed column = kolumna wyliczana
            //stored: true - wartość jest przechowywana w bazie danych, a nie wyliczana przy każdym odczycie
            builder.Property(x => x.TotalValue).HasComputedColumnSql("[Value] * (1 + [Tax])", stored: true);
            //nie każda kolumna wyliczana może mieć flagę stored: true
            //aby było to możliwe dane muszą być deterministyczne, czyli zawsze zwracać ten sam wynik dla tych samych danych wejściowych
            builder.Property<DateTime>("CurrentDate").HasComputedColumnSql("GETDATE()");

            builder.Property(x => x.IsExpired).HasComputedColumnSql("CASE WHEN [OrderDate] < GETDATE() THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END");

            builder.Property(x => x.Type).HasConversion(
                   v => v.ToString(),
                   v => Enum.Parse<OrderType>(v)
                );

            //builder.Property(x => x.Parameters).HasConversion(new EnumToStringConverter<OrderParameters>());
            builder.Property(x => x.Parameters).HasConversion<string>();

            builder.Property(x => x.Name).HasConversion(new ObfuscationConverter());
        }
    }
}
