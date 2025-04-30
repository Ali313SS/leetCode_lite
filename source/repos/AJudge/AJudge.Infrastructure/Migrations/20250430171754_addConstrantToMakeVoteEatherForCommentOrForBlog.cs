using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJudge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addConstrantToMakeVoteEatherForCommentOrForBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Vote_CommentOrBlog",
                table: "Votes",
                sql: "(CommentId IS NOT NULL AND BlogId IS NULL) OR (CommentId IS NULL AND BlogId IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Vote_CommentOrBlog",
                table: "Votes");
        }
    }
}
