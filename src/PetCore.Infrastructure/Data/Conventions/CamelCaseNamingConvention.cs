using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PetCore.Infrastructure.Data.Conventions;

public static class CamelCaseNamingExtensions
{
    public static void ApplyNomenclaturasCamelCase(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Tabela em camelCase
            var tableName = ToCamelCase(entity.GetTableName() ?? entity.ClrType.Name);
            entity.SetTableName(tableName);

            // Colunas em camelCase
            foreach (var property in entity.GetProperties())
            {
                var columnName = ToCamelCase(property.GetColumnName(StoreObjectIdentifier.Table(tableName, entity.GetSchema())));
                property.SetColumnName(columnName);
            }

            // FKs em camelCase
            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (keyName != null)
                    key.SetName(ToCamelCase(keyName));
            }

            // Índices em camelCase
            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (indexName != null)
                    index.SetDatabaseName(ToCamelCase(indexName));
            }
        }
    }

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        if (name.Length == 1) return name.ToLower();
        return char.ToLowerInvariant(name[0]) + name[1..];
    }
}
