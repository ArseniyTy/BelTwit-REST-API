using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BelTwit_REST_API.Migrations
{
    public partial class v03 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Action = table.Column<string>(nullable: true),
                    LogLevel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
