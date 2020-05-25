using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BelTwit_REST_API.Migrations
{
    public partial class v065 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserIdRetweetedFrom",
                table: "Tweets",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserIdRetweetedFrom",
                table: "Tweets",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
