using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DATN.Migrations
{
    public partial class upSlide : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "SlideShow",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "SlideShow");
        }
    }
}
