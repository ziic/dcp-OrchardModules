using dcp.Utility.Models;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace dcp.Utility
{
    [OrchardFeature("dcp.Routing")]
    public class Migrations : DataMigrationImpl
    {

        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ExtendedAliasRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<int>("AliasRecord_Id", column => column.NotNull())
                .Column<string>("RouteName", column => column
                    .NotNull()
                    .WithLength(50))
            );

            SchemaBuilder.CreateForeignKey("ExtendedAliasRecord_AliasRecord", "ExtendedAliasRecord", new[] { "AliasRecord_Id" }, "Orchard.Alias", "AliasRecord", new[] { "Id" });

            return 1;
        }

    }
}