using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aff.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAttributionWindow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttributionWindowDays",
                table: "Campaigns",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttributionWindowDays",
                table: "Campaigns");
        }
    }
}
