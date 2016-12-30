using System;
using Orchard.Data.Migration;

namespace dcp.Routing
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

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable("RedirectRule", table => table
                .AddColumn<bool>("IsPermanent", c => c.NotNull().WithDefault(false))
                );

            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable("RedirectRule", table => table
                .AddColumn<DateTime>("CreatedDateTime", c => c.NotNull().WithDefault("GetDate()"))
                );

            return 3;
        }
    }
}