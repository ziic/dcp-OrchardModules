using System;
using Orchard.Data.Migration;

namespace dcp.Routing.Redirects
{
    public class Migration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("RedirectRule", table => table
                .Column<int>("Id", c => c
                    .PrimaryKey()
                    .Identity())
                .Column<DateTime>("CreatedDateTime", c => c.NotNull().WithDefault("GetDate()"))
                .Column<string>("SourceUrl", c => c.NotNull().WithDefault(""))
                .Column<string>("DestinationUrl", c => c.NotNull().WithDefault(""))
                .Column<bool>("IsPermanent", c => c.NotNull().WithDefault(false))
                );

            return 1;
        }
    }
}