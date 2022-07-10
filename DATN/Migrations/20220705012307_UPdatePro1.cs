using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DATN.Migrations
{
    public partial class UPdatePro1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "SlideShow");

            migrationBuilder.RenameColumn(
                name: "Quantily",
                table: "Product",
                newName: "Stock");

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "SlideShow",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Product",
                newName: "Quantily");

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "SlideShow",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "SlideShow",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
