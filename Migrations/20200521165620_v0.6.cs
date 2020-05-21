using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BelTwit_REST_API.Migrations
{
    public partial class v06 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    TweetId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Tweets_TweetId",
                        column: x => x.TweetId,
                        principalTable: "Tweets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRateStates",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    TweetId = table.Column<Guid>(nullable: false),
                    RateState = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRateStates", x => new { x.UserId, x.TweetId });
                    table.ForeignKey(
                        name: "FK_UserRateStates_Tweets_TweetId",
                        column: x => x.TweetId,
                        principalTable: "Tweets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TweetId",
                table: "Comments",
                column: "TweetId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRateStates_TweetId",
                table: "UserRateStates",
                column: "TweetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "UserRateStates");

            migrationBuilder.AddColumn<string>(
                name: "CommentsDb",
                table: "Tweets",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
