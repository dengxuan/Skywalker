using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Skywalker.Transfer.EntityFrameworkCore.DbMigrations.Migrations
{
    public partial class Initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TsfrMerchants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Scheme = table.Column<string>(type: "varchar(20) CHARACTER SET utf8mb4", maxLength: 20, nullable: false),
                    Number = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "varchar(64) CHARACTER SET utf8mb4", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: false),
                    CipherKey = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Address = table.Column<string>(type: "varchar(256) CHARACTER SET utf8mb4", maxLength: 256, nullable: false),
                    MerchantType = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(40) CHARACTER SET utf8mb4", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TsfrMerchants", x => new { x.Id, x.Scheme, x.Number });
                });

            migrationBuilder.CreateTable(
                name: "TsfrTraders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TraderType = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(40) CHARACTER SET utf8mb4", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TsfrTraders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TsfrTradeOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    MerchantId = table.Column<Guid>(type: "char(36)", nullable: false),
                    MerchantScheme = table.Column<string>(type: "varchar(20) CHARACTER SET utf8mb4", nullable: false),
                    MerchantNumber = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", nullable: false),
                    TraderId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    HandlingFee = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Withholding = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TradeOrderType = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TradeAuditedType = table.Column<int>(type: "int", nullable: false),
                    RevokeTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RevokeReason = table.Column<string>(type: "varchar(1000) CHARACTER SET utf8mb4", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TsfrTradeOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TsfrTradeOrders_TsfrMerchants_MerchantId_MerchantScheme_Merc~",
                        columns: x => new { x.MerchantId, x.MerchantScheme, x.MerchantNumber },
                        principalTable: "TsfrMerchants",
                        principalColumns: new[] { "Id", "Scheme", "Number" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TsfrTradeOrders_TsfrTraders_TraderId",
                        column: x => x.TraderId,
                        principalTable: "TsfrTraders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TsfrTransferDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TraderId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TransferType = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Message = table.Column<string>(type: "varchar(1000) CHARACTER SET utf8mb4", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TsfrTransferDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TsfrTransferDetails_TsfrTraders_TraderId",
                        column: x => x.TraderId,
                        principalTable: "TsfrTraders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TsfrTradeOrders_MerchantId_MerchantScheme_MerchantNumber",
                table: "TsfrTradeOrders",
                columns: new[] { "MerchantId", "MerchantScheme", "MerchantNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_TsfrTradeOrders_TraderId",
                table: "TsfrTradeOrders",
                column: "TraderId");

            migrationBuilder.CreateIndex(
                name: "IX_TsfrTransferDetails_TraderId",
                table: "TsfrTransferDetails",
                column: "TraderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TsfrTradeOrders");

            migrationBuilder.DropTable(
                name: "TsfrTransferDetails");

            migrationBuilder.DropTable(
                name: "TsfrMerchants");

            migrationBuilder.DropTable(
                name: "TsfrTraders");
        }
    }
}
