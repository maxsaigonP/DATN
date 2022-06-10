using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DATN.Migrations
{
    public partial class updateTrad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<double>(
                name: "Star",
                table: "Product",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "TradeMark",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Category",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TradeMark",
                table: "Product");

            migrationBuilder.AlterColumn<double>(
                name: "Star",
                table: "Product",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TradeMarkId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Category",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

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
    }
}
