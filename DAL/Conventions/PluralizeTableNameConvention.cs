using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Conventions
{
    internal class PluralizeTableNameConvention : IModelFinalizingConvention
    {
        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
        {
            modelBuilder.Metadata.GetEntityTypes()
                //gettablename - ustawiona nazwa tabeli, getdefaulttablename - domyślna nazwa tabeli, która jest taka sama jak nazwa klasy
                //umożliwamy ustawienie nazwy w konfiguracji encji, ale jeśli nie została ona ustawiona, to pluralizujemy domyślną nazwę tabeli
                .Where(x => x.GetDefaultTableName() == x.GetTableName())
                    .ToList()
                    //pluralizujemy nazwy tabel
                    .ForEach(x => x.SetTableName(new Pluralize.NET.Core.Pluralizer().Pluralize(x.GetDefaultTableName())));
                }
    }
}
