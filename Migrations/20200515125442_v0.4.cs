using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BelTwit_REST_API.Migrations
{
    public partial class v04 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubscriberSubscriptions",
                columns: table => new
                {
                    WhoSubscribeId = table.Column<Guid>(nullable: false),
                    OnWhomSubscribeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberSubscriptions", x => new { x.WhoSubscribeId, x.OnWhomSubscribeId });
                    table.ForeignKey(
                        name: "FK_SubscriberSubscriptions_Users_OnWhomSubscribeId",
                        column: x => x.OnWhomSubscribeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriberSubscriptions_Users_WhoSubscribeId",
                        column: x => x.WhoSubscribeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriberSubscriptions_OnWhomSubscribeId",
                table: "SubscriberSubscriptions",
                column: "OnWhomSubscribeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriberSubscriptions");
        }
    }
}
