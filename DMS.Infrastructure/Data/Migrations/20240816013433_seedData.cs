using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class seedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "NID", "Role", "email", "password" },
                values: new object[,]
                {
                    { 1, "1234567890", 0, "admin@example.com", "AdminPassword123" },
                    { 2, "0987654321", 1, "user@example.com", "UserPassword123" }
                });

            migrationBuilder.InsertData(
                table: "Workspaces",
                columns: new[] { "Id", "Name", "UserId" },
                values: new object[] { 1, "Default Workspace", 1 });

            migrationBuilder.InsertData(
                table: "Directories",
                columns: new[] { "Id", "Name", "WorkspaceId" },
                values: new object[] { 1, "Root Directory", 1 });

            migrationBuilder.InsertData(
                table: "Documents",
                columns: new[] { "Id", "DirectoryId", "Name" },
                values: new object[] { 1, 1, "Sample Document" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Documents",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Directories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Workspaces",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
