using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    message = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notification_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications_id", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tasks_name",
                table: "tasks",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tables_name",
                table: "tables",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_name",
                table: "notifications",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_tasks_name",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tables_name",
                table: "tables");
        }
    }
}
