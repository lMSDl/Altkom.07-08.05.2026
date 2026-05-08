using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Configuration
{
    internal class OrderSummaryConfiguration : IEntityTypeConfiguration<Models.OrderSummary>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Models.OrderSummary> builder)
        {
            builder.ToView("View_OrderSummary");
        }
    }
}
