using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DATN.Migrations
{
    public partial class updateProBrand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TradeMark",
                table: "Product");

            migrationBuilder.AddColumn<int>(
                name: "TradeMarkId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Product_TradeMarkId",
                table: "Product",
                column: "TradeMarkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_TradeMarks_TradeMarkId",
                table: "Product",
                column: "TradeMarkId",
                principalTable: "TradeMarks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_TradeMarks_TradeMarkId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_TradeMarkId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "TradeMarkId",
                table: "Product");

            migrationBuilder.AddColumn<string>(
                name: "TradeMark",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
