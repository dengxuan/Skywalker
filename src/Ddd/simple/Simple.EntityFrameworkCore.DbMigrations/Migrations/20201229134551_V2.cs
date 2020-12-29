using Microsoft.EntityFrameworkCore.Migrations;

namespace Simple.EntityFrameworkCore.DbMigrations.Migrations
{
    public partial class V2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOrder_SmpUserValues_UserValueId",
                table: "UserOrder");

            migrationBuilder.DropIndex(
                name: "IX_UserOrder_UserValueId",
                table: "UserOrder");

            migrationBuilder.DropColumn(
                name: "UserValueId",
                table: "UserOrder");

            migrationBuilder.AddColumn<int>(
                name: "UserOrderId",
                table: "SmpUserValues",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmpUserValues_UserOrderId",
                table: "SmpUserValues",
                column: "UserOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_SmpUserValues_UserOrder_UserOrderId",
                table: "SmpUserValues",
                column: "UserOrderId",
                principalTable: "UserOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmpUserValues_UserOrder_UserOrderId",
                table: "SmpUserValues");

            migrationBuilder.DropIndex(
                name: "IX_SmpUserValues_UserOrderId",
                table: "SmpUserValues");

            migrationBuilder.DropColumn(
                name: "UserOrderId",
                table: "SmpUserValues");

            migrationBuilder.AddColumn<int>(
                name: "UserValueId",
                table: "UserOrder",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserOrder_UserValueId",
                table: "UserOrder",
                column: "UserValueId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrder_SmpUserValues_UserValueId",
                table: "UserOrder",
                column: "UserValueId",
                principalTable: "SmpUserValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
