using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreetSweepingReminder.Api.Migrations
{
    /// <inheritdoc />
    public partial class SideOfStreetToReminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SideOfStreet",
                table: "Reminders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SideOfStreet",
                table: "Reminders");
        }
    }
}
