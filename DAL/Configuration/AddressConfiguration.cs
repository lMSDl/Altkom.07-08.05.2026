using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace DAL.Configuration
{
    internal class AddressConfiguration : EntityConfiguration<Models.Address>
    {
        public override void Configure(EntityTypeBuilder<Address> builder)
        {
            base.Configure(builder);
        }
    }
}
