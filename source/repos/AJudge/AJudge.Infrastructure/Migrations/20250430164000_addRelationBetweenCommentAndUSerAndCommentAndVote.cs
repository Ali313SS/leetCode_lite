using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJudge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addRelationBetweenCommentAndUSerAndCommentAndVote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Blog_BlogId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Blog_BlogId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_UserId",
                table: "Votes");

            migrationBuilder.AlterColumn<int>(
                name: "BlogId",
                table: "Votes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "CommentId",
                table: "Votes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Comment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_CommentId",
                table: "Votes",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId_BlogId",
                table: "Votes",
                columns: new[] { "UserId", "BlogId" },
                unique: true,
                filter: "[BlogId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId_CommentId",
                table: "Votes",
                columns: new[] { "UserId", "CommentId" },
                unique: true,
                filter: "[CommentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserId",
                table: "Comment",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Blog_BlogId",
                table: "Comment",
                column: "BlogId",
                principalTable: "Blog",
                principalColumn: "BlogId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Users_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Blog_BlogId",
                table: "Votes",
                column: "BlogId",
                principalTable: "Blog",
                principalColumn: "BlogId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Comment_CommentId",
                table: "Votes",
                column: "CommentId",
                principalTable: "Comment",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Blog_BlogId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Users_UserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Blog_BlogId",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Comment_CommentId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_CommentId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_UserId_BlogId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_UserId_CommentId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Comment_UserId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Comment");

            migrationBuilder.AlterColumn<int>(
                name: "BlogId",
                table: "Votes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId",
                table: "Votes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Blog_BlogId",
                table: "Comment",
                column: "BlogId",
                principalTable: "Blog",
                principalColumn: "BlogId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Blog_BlogId",
                table: "Votes",
                column: "BlogId",
                principalTable: "Blog",
                principalColumn: "BlogId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
