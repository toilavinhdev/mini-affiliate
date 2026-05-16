using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aff.API.Migrations
{
    /// <inheritdoc />
    public partial class _0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AffiliateLinks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CampaignId = table.Column<string>(type: "TEXT", nullable: false),
                    PartnerId = table.Column<string>(type: "TEXT", nullable: false),
                    TrackingCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    TargetUrl = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Label = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalClicks = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalConversions = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    PartnerId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ServiceType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CommissionType = table.Column<int>(type: "INTEGER", nullable: false),
                    CommissionValue = table.Column<decimal>(type: "REAL", nullable: false),
                    MaxBudget = table.Column<decimal>(type: "REAL", nullable: true),
                    SpentBudget = table.Column<decimal>(type: "REAL", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<string>(type: "TEXT", nullable: false),
                    EndDate = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clicks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    TrackingCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CampaignId = table.Column<string>(type: "TEXT", nullable: false),
                    PartnerId = table.Column<string>(type: "TEXT", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Referer = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    ClickedAt = table.Column<string>(type: "TEXT", nullable: false),
                    ConvertedAt = table.Column<string>(type: "TEXT", nullable: true),
                    ConversionId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clicks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Conversions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ClickId = table.Column<string>(type: "TEXT", nullable: true),
                    TrackingCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CampaignId = table.Column<string>(type: "TEXT", nullable: false),
                    PartnerId = table.Column<string>(type: "TEXT", nullable: false),
                    ServiceType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ServiceTransactionId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    EndUserId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TransactionAmount = table.Column<decimal>(type: "REAL", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "REAL", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    RejectionReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<string>(type: "TEXT", nullable: false),
                    ApprovedAt = table.Column<string>(type: "TEXT", nullable: true),
                    SettlementId = table.Column<string>(type: "TEXT", nullable: true),
                    SettledAt = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Partners",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    BusinessName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ContactEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    TaxCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BankName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    BankAccountNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BankAccountHolder = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    RejectionReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ApprovedAt = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settlements",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    PartnerId = table.Column<string>(type: "TEXT", nullable: false),
                    PeriodStart = table.Column<string>(type: "TEXT", nullable: false),
                    PeriodEnd = table.Column<string>(type: "TEXT", nullable: false),
                    TotalConversions = table.Column<int>(type: "INTEGER", nullable: false),
                    GrossCommission = table.Column<decimal>(type: "REAL", nullable: false),
                    TaxRate = table.Column<decimal>(type: "REAL", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "REAL", nullable: false),
                    NetCommission = table.Column<decimal>(type: "REAL", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentReference = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessedAt = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settlements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SettlementItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    SettlementId = table.Column<string>(type: "TEXT", nullable: false),
                    ConversionId = table.Column<string>(type: "TEXT", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettlementItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SettlementItems_Settlements_SettlementId",
                        column: x => x.SettlementId,
                        principalTable: "Settlements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AffiliateLinks_TrackingCode",
                table: "AffiliateLinks",
                column: "TrackingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clicks_TrackingCode",
                table: "Clicks",
                column: "TrackingCode");

            migrationBuilder.CreateIndex(
                name: "IX_Conversions_ServiceTransactionId",
                table: "Conversions",
                column: "ServiceTransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partners_ContactEmail",
                table: "Partners",
                column: "ContactEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SettlementItems_SettlementId",
                table: "SettlementItems",
                column: "SettlementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AffiliateLinks");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "Clicks");

            migrationBuilder.DropTable(
                name: "Conversions");

            migrationBuilder.DropTable(
                name: "Partners");

            migrationBuilder.DropTable(
                name: "SettlementItems");

            migrationBuilder.DropTable(
                name: "Settlements");
        }
    }
}
