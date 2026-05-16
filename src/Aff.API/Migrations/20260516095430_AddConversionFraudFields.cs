using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aff.API.Migrations
{
    /// <inheritdoc />
    public partial class AddConversionFraudFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FraudReason",
                table: "Conversions",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuspicious",
                table: "Conversions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FraudReason",
                table: "Conversions");

            migrationBuilder.DropColumn(
                name: "IsSuspicious",
                table: "Conversions");
        }
    }
}
