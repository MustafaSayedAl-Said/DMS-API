using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ActionLogsUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userName",
                table: "ActionLogs",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "ActionLogs",
                newName: "CreationDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "ActionLogs",
                newName: "userName");

            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "ActionLogs",
                newName: "Timestamp");
        }
    }
}
