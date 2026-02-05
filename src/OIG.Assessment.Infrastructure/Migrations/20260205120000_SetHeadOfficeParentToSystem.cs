using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OIG.Assessment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetHeadOfficeParentToSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Organizations"" AS ""HeadOffice""
                SET ""ParentId"" = ""System"".""Id""
                FROM ""Organizations"" AS ""System""
                WHERE ""System"".""Name"" = 'System'
                  AND ""HeadOffice"".""Name"" = 'Head Office'
                  AND ""HeadOffice"".""ParentId"" IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Organizations""
                SET ""ParentId"" = NULL
                WHERE ""Name"" = 'Head Office'
                  AND ""ParentId"" IN (SELECT ""Id"" FROM ""Organizations"" WHERE ""Name"" = 'System');
            ");
        }
    }
}
