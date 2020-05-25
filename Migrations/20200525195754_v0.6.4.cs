using Microsoft.EntityFrameworkCore.Migrations;

namespace BelTwit_REST_API.Migrations
{
    public partial class v064 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tweets_TweetId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tweets_TweetId",
                table: "Comments",
                column: "TweetId",
                principalTable: "Tweets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tweets_TweetId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tweets_TweetId",
                table: "Comments",
                column: "TweetId",
                principalTable: "Tweets",
                principalColumn: "Id");
        }
    }
}
