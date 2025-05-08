using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJudge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class KosamElProjectDa2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProblemTag_OrignalProblems_OrignalProblemsProblemSourceID_OrignalProblemsProblemId",
                table: "ProblemTag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_OrignalProblems_OrignalProblemsProblemSourceID_OrignalProblemsProblemId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCase_OrignalProblems_OrignalProblemsProblemSourceID_OrignalProblemsProblemId",
                table: "TestCase");

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
                name: "PK_OrignalProblems",
                table: "OrignalProblems");

            migrationBuilder.DropIndex(
                name: "IX_OrignalProblems_ProblemId_ProblemSourceID",
                table: "OrignalProblems");

            migrationBuilder.DropColumn(
                name: "OrignalProblemsProblemId",
                table: "TestCase");

            migrationBuilder.DropColumn(
                name: "OrignalProblemsProblemSourceID",
                table: "TestCase");

            migrationBuilder.DropColumn(
                name: "OrignalProblemsProblemSourceID",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "OrignalProblemsProblemSourceID",
                table: "ProblemTag");

            migrationBuilder.DropColumn(
                name: "ProblemSourceID",
                table: "OrignalProblems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrignalProblems",
                table: "OrignalProblems",
                column: "ProblemId");

            migrationBuilder.CreateTable(
                name: "OrignalTestCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Input = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Output = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProblemId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrignalTestCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrignalTestCases_OrignalProblems_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "OrignalProblems",
                        principalColumn: "ProblemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_OrignalProblemsProblemId",
                table: "Tags",
                column: "OrignalProblemsProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemTag_OrignalProblemsProblemId",
                table: "ProblemTag",
                column: "OrignalProblemsProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrignalProblems_ProblemId",
                table: "OrignalProblems",
                column: "ProblemId",
                unique: true,
                filter: "[ProblemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrignalTestCases_ProblemId",
                table: "OrignalTestCases",
                column: "ProblemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemTag_OrignalProblems_OrignalProblemsProblemId",
                table: "ProblemTag",
                column: "OrignalProblemsProblemId",
                principalTable: "OrignalProblems",
                principalColumn: "ProblemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_OrignalProblems_OrignalProblemsProblemId",
                table: "Tags",
                column: "OrignalProblemsProblemId",
                principalTable: "OrignalProblems",
                principalColumn: "ProblemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProblemTag_OrignalProblems_OrignalProblemsProblemId",
                table: "ProblemTag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_OrignalProblems_OrignalProblemsProblemId",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "OrignalTestCases");

            migrationBuilder.DropIndex(
                name: "IX_Tags_OrignalProblemsProblemId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_ProblemTag_OrignalProblemsProblemId",
                table: "ProblemTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrignalProblems",
                table: "OrignalProblems");

            migrationBuilder.DropIndex(
                name: "IX_OrignalProblems_ProblemId",
                table: "OrignalProblems");

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

            migrationBuilder.AddColumn<int>(
                name: "OrignalProblemsProblemSourceID",
                table: "Tags",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrignalProblemsProblemSourceID",
                table: "ProblemTag",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProblemSourceID",
                table: "OrignalProblems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrignalProblems",
                table: "OrignalProblems",
                columns: new[] { "ProblemSourceID", "ProblemId" });

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
        }
    }
}
