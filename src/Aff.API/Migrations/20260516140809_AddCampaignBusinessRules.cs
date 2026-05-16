using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aff.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignBusinessRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MaxCommissionPerConversion",
                table: "Campaigns",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinOrderAmount",
                table: "Campaigns",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxCommissionPerConversion",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "MinOrderAmount",
                table: "Campaigns");
        }
    }
}
