using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GexPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OptionChains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ticker = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UnderlyingPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionChains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OptionContracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StrikePrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    OptionType = table.Column<string>(type: "TEXT", maxLength: 4, nullable: false),
                    BidPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    AskPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    LastPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: true),
                    OpenInterest = table.Column<long>(type: "INTEGER", nullable: false),
                    Volume = table.Column<long>(type: "INTEGER", nullable: false),
                    ImpliedVolatility = table.Column<decimal>(type: "TEXT", precision: 10, scale: 6, nullable: false),
                    Delta = table.Column<decimal>(type: "TEXT", precision: 10, scale: 6, nullable: true),
                    Gamma = table.Column<decimal>(type: "TEXT", precision: 10, scale: 6, nullable: true),
                    Theta = table.Column<decimal>(type: "TEXT", precision: 10, scale: 6, nullable: true),
                    Vega = table.Column<decimal>(type: "TEXT", precision: 10, scale: 6, nullable: true),
                    Rho = table.Column<decimal>(type: "TEXT", precision: 10, scale: 6, nullable: true),
                    OptionChainId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionContracts_OptionChains_OptionChainId",
                        column: x => x.OptionChainId,
                        principalTable: "OptionChains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OptionChains_Ticker_ExpirationDate",
                table: "OptionChains",
                columns: new[] { "Ticker", "ExpirationDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OptionContract_ChainTypeStrike",
                table: "OptionContracts",
                columns: new[] { "OptionChainId", "OptionType", "StrikePrice" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OptionContracts");

            migrationBuilder.DropTable(
                name: "OptionChains");
        }
    }
}
