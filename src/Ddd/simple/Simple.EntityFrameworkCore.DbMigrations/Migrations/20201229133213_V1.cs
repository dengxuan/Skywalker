using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Simple.EntityFrameworkCore.DbMigrations.Migrations
{
    public partial class V1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SmpUserValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(40) CHARACTER SET utf8mb4", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmpUserValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    UserValueId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<short>(type: "smallint", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(40) CHARACTER SET utf8mb4", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOrder_SmpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "SmpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOrder_SmpUserValues_UserValueId",
                        column: x => x.UserValueId,
                        principalTable: "SmpUserValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserOrder_UserId",
                table: "UserOrder",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrder_UserValueId",
                table: "UserOrder",
                column: "UserValueId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserOrder");

            migrationBuilder.DropTable(
                name: "SmpUserValues");
        }
    }
}
