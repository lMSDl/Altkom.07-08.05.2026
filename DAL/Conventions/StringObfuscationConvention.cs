using DAL.Converters;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Conventions
{
    internal class StringObfuscationConvention : IModelFinalizingConvention
    {
        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
        {
            foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string))
                    {
                        //ustawiamy konwersję dla wszystkich właściwości typu string, która będzie obfuskować dane podczas zapisu do bazy i odszyfrowywać podczas odczytu
                        property.SetValueConverter(new ObfuscationConverter());
                    }
                }
            }
        }
    }
}
