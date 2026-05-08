using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Configuration
{
    internal class AuthDataConfiguration : IEntityTypeConfiguration<AuthData>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<AuthData> builder)
        {
            //builder.HasKey(x => x.Key);
            builder.ToTable("Logins");
        }
    }
}
