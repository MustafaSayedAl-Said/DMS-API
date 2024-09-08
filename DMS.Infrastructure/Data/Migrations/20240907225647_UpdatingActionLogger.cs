using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingActionLogger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionLogs_Workspaces_WorkspaceId",
                table: "ActionLogs");

            migrationBuilder.DropIndex(
                name: "IX_ActionLogs_WorkspaceId",
                table: "ActionLogs");

            migrationBuilder.DropColumn(
                name: "WorkspaceId",
                table: "ActionLogs");

            migrationBuilder.DropColumn(
                name: "WorkspaceName",
                table: "ActionLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkspaceId",
                table: "ActionLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WorkspaceName",
                table: "ActionLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_WorkspaceId",
                table: "ActionLogs",
                column: "WorkspaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionLogs_Workspaces_WorkspaceId",
                table: "ActionLogs",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
