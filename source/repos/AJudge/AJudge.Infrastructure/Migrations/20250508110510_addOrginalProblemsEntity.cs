using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJudge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addOrginalProblemsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Blog_BlogId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Users_UserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Comment_CommentId",
                table: "Votes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_UserId",
                table: "Comments",
                newName: "IX_Comments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_BlogId",
                table: "Comments",
                newName: "IX_Comments_BlogId");

            migrationBuilder.AddColumn<string>(
                name: "OrignalProblemsProblemId",
                table: "TestCase",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrignalProblemsProblemSourceID",
                table: "TestCase",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrignalProblemsProblemId",
                table: "Tags",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrignalProblemsProblemSourceID",
                table: "Tags",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrignalProblemsProblemId",
                table: "ProblemTag",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrignalProblemsProblemSourceID",
                table: "ProblemTag",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BlogId",
                table: "Comments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "OrignalProblems",
                columns: table => new
                {
                    ProblemId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProblemSourceID = table.Column<int>(type: "int", nullable: false),
                    ProblemSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProblemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProblemLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InputFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    numberOfTestCases = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrignalProblems", x => new { x.ProblemSourceID, x.ProblemId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestCase_OrignalProblemsProblemSourceID_OrignalProblemsProblemId",
                table: "TestCase",
                columns: new[] { "OrignalProblemsProblemSourceID", "OrignalProblemsProblemId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_OrignalProblemsProblemSourceID_OrignalProblemsProblemId",
                table: "Tags",
                columns: new[] { "OrignalProblemsProblemSourceID", "OrignalProblemsProblemId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProblemTag_OrignalProblemsProblemSourceID_OrignalProblemsProblemId",
                table: "ProblemTag",
                columns: new[] { "OrignalProblemsProblemSourceID", "OrignalProblemsProblemId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrignalProblems_ProblemId_ProblemSourceID",
                table: "OrignalProblems",
                columns: new[] { "ProblemSourceID", "ProblemId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Blog_BlogId",
                table: "Comments",
                column: "BlogId",
                principalTable: "Blog",
                principalColumn: "BlogId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemTag_OrignalProblems_OrignalProblemsProblemSourceID_OrignalProblemsProblemId",
                table: "ProblemTag",
                columns: new[] { "OrignalProblemsProblemSourceID", "OrignalProblemsProblemId" },
                principalTable: "OrignalProblems",
                principalColumns: new[] { "ProblemSourceID", "ProblemId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_OrignalProblems_OrignalProblemsProblemSourceID_OrignalProblemsProblemId",
                table: "Tags",
                columns: new[] { "OrignalProblemsProblemSourceID", "OrignalProblemsProblemId" },
                principalTable: "OrignalProblems",
                principalColumns: new[] { "ProblemSourceID", "ProblemId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TestCase_OrignalProblems_OrignalProblemsProblemSourceID_OrignalProblemsProblemId",
                table: "TestCase",
                columns: new[] { "OrignalProblemsProblemSourceID", "OrignalProblemsProblemId" },
                principalTable: "OrignalProblems",
                principalColumns: new[] { "ProblemSourceID", "ProblemId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Comments_CommentId",
                table: "Votes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Blog_BlogId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_ProblemTag_OrignalProblems_OrignalProblemsProblemSourceID_OrignalProblemsProblemId",
                table: "ProblemTag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_OrignalProblems_OrignalProblemsProblemSourceID_OrignalProblemsProblemId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCase_OrignalProblems_OrignalProblemsProblemSourceID_OrignalProblemsProblemId",
                table: "TestCase");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Comments_CommentId",
                table: "Votes");

            migrationBuilder.DropTable(
                name: "OrignalProblems");

            migrationBuilder.DropIndex(
                name: "IX_TestCase_OrignalProblemsProblemSourceID_OrignalProblemsProblemId",
                table: "TestCase");

            migrationBuilder.DropIndex(
                name: "IX_Tags_OrignalProblemsProblemSourceID_OrignalProblemsProblemId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_ProblemTag_OrignalProblemsProblemSourceID_OrignalProblemsProblemId",
                table: "ProblemTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "OrignalProblemsProblemId",
                table: "TestCase");

            migrationBuilder.DropColumn(
                name: "OrignalProblemsProblemSourceID",
                table: "TestCase");

            migrationBuilder.DropColumn(
                name: "OrignalProblemsProblemId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "OrignalProblemsProblemSourceID",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "OrignalProblemsProblemId",
                table: "ProblemTag");

            migrationBuilder.DropColumn(
                name: "OrignalProblemsProblemSourceID",
                table: "ProblemTag");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserId",
                table: "Comment",
                newName: "IX_Comment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_BlogId",
                table: "Comment",
                newName: "IX_Comment_BlogId");

            migrationBuilder.AlterColumn<int>(
                name: "BlogId",
                table: "Comment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Blog_BlogId",
                table: "Comment",
                column: "BlogId",
                principalTable: "Blog",
                principalColumn: "BlogId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Users_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Comment_CommentId",
                table: "Votes",
                column: "CommentId",
                principalTable: "Comment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
