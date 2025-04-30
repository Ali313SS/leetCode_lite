using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJudge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class makeCommentVoteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Comment_CommentId",
                table: "Votes");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Comment_CommentId",
                table: "Votes",
                column: "CommentId",
                principalTable: "Comment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Comment_CommentId",
                table: "Votes");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Comment_CommentId",
                table: "Votes",
                column: "CommentId",
                principalTable: "Comment",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
