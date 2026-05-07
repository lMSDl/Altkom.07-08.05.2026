using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace DAL.Configuration
{
    internal class ProductConfiguration : EntityConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.Order).WithMany(x => x.Products);
        }
    }
}
