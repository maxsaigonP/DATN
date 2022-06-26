using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DATN.Migrations
{
    public partial class udSlide : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SlideShow_Product_ProductId",
                table: "SlideShow");

            migrationBuilder.DropIndex(
                name: "IX_SlideShow_ProductId",
                table: "SlideShow");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "SlideShow");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SlideShow",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SlideShow",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "SlideShow",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SlideShow_ProductId",
                table: "SlideShow",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_SlideShow_Product_ProductId",
                table: "SlideShow",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
