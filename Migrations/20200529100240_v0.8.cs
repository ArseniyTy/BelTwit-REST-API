using Microsoft.EntityFrameworkCore.Migrations;

namespace BelTwit_REST_API.Migrations
{
    public partial class v08 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Logs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                table: "Logs");
        }
    }
}
