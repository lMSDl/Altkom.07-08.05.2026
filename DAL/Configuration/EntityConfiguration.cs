using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace DAL.Configuration
{
    internal abstract class EntityConfiguration<T> : IEntityTypeConfiguration<T> where T : Entity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
        }
    }
}
