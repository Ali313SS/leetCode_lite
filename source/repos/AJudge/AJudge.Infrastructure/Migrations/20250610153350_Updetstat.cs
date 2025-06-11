using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJudge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updetstat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContestProblems_Contests_ContestId",
                table: "ContestProblems");

            migrationBuilder.DropForeignKey(
                name: "FK_ContestProblems_Problems_ProblemId",
                table: "ContestProblems");

            migrationBuilder.DropForeignKey(
                name: "FK_Problems_Contests_ContestId",
                table: "Problems");

            migrationBuilder.DropIndex(
                name: "IX_Problems_ContestId",
                table: "Problems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContestProblems",
                table: "ContestProblems");

            migrationBuilder.RenameTable(
                name: "ContestProblems",
                newName: "ContestProblem");

            migrationBuilder.RenameIndex(
                name: "IX_ContestProblems_ProblemId",
                table: "ContestProblem",
                newName: "IX_ContestProblem_ProblemId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Contests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContestProblem",
                table: "ContestProblem",
                columns: new[] { "ContestId", "ProblemId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ContestProblem_Contests_ContestId",
                table: "ContestProblem",
                column: "ContestId",
                principalTable: "Contests",
                principalColumn: "ContestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContestProblem_Problems_ProblemId",
                table: "ContestProblem",
                column: "ProblemId",
                principalTable: "Problems",
                principalColumn: "ProblemId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContestProblem_Contests_ContestId",
                table: "ContestProblem");

            migrationBuilder.DropForeignKey(
                name: "FK_ContestProblem_Problems_ProblemId",
                table: "ContestProblem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContestProblem",
                table: "ContestProblem");

            migrationBuilder.RenameTable(
                name: "ContestProblem",
                newName: "ContestProblems");

            migrationBuilder.RenameIndex(
                name: "IX_ContestProblem_ProblemId",
                table: "ContestProblems",
                newName: "IX_ContestProblems_ProblemId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Contests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContestProblems",
                table: "ContestProblems",
                columns: new[] { "ContestId", "ProblemId" });

            migrationBuilder.CreateIndex(
                name: "IX_Problems_ContestId",
                table: "Problems",
                column: "ContestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContestProblems_Contests_ContestId",
                table: "ContestProblems",
                column: "ContestId",
                principalTable: "Contests",
                principalColumn: "ContestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContestProblems_Problems_ProblemId",
                table: "ContestProblems",
                column: "ProblemId",
                principalTable: "Problems",
                principalColumn: "ProblemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_Contests_ContestId",
                table: "Problems",
                column: "ContestId",
                principalTable: "Contests",
                principalColumn: "ContestId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
