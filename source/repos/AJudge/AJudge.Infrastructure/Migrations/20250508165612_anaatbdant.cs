using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJudge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class anaatbdant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCase_Problems_ProblemId",
                table: "TestCase");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestCase",
                table: "TestCase");

            migrationBuilder.RenameTable(
                name: "TestCase",
                newName: "TestCases");

            migrationBuilder.RenameIndex(
                name: "IX_TestCase_ProblemId",
                table: "TestCases",
                newName: "IX_TestCases_ProblemId");

            migrationBuilder.AlterColumn<string>(
                name: "ProblemSource",
                table: "Problems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestCases",
                table: "TestCases",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCases_Problems_ProblemId",
                table: "TestCases",
                column: "ProblemId",
                principalTable: "Problems",
                principalColumn: "ProblemId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCases_Problems_ProblemId",
                table: "TestCases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestCases",
                table: "TestCases");

            migrationBuilder.RenameTable(
                name: "TestCases",
                newName: "TestCase");

            migrationBuilder.RenameIndex(
                name: "IX_TestCases_ProblemId",
                table: "TestCase",
                newName: "IX_TestCase_ProblemId");

            migrationBuilder.AlterColumn<string>(
                name: "ProblemSource",
                table: "Problems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestCase",
                table: "TestCase",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCase_Problems_ProblemId",
                table: "TestCase",
                column: "ProblemId",
                principalTable: "Problems",
                principalColumn: "ProblemId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
