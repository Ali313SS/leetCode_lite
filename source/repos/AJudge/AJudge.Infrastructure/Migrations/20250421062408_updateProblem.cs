using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJudge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateProblem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProblemTag_Problems_ProblemsProblemId",
                table: "ProblemTag");

            migrationBuilder.DropForeignKey(
                name: "FK_ProblemTag_Tags_TagsTagId",
                table: "ProblemTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProblemTag",
                table: "ProblemTag");

            migrationBuilder.DropColumn(
                name: "SampleInput",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "SampleOutput",
                table: "Problems");

            migrationBuilder.RenameTable(
                name: "ProblemTag",
                newName: "ProblemTags");

            migrationBuilder.RenameIndex(
                name: "IX_ProblemTag_TagsTagId",
                table: "ProblemTags",
                newName: "IX_ProblemTags_TagsTagId");

            migrationBuilder.AddColumn<int>(
                name: "numberOfTestCases",
                table: "Problems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProblemTags",
                table: "ProblemTags",
                columns: new[] { "ProblemsProblemId", "TagsTagId" });

            migrationBuilder.CreateTable(
                name: "InputOutputTestCase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProblemId = table.Column<int>(type: "int", nullable: false),
                    TestCaseId = table.Column<int>(type: "int", nullable: false),
                    Input = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Output = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputOutputTestCase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProblemTestCases_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problems",
                        principalColumn: "ProblemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InputOutputTestCase_ProblemId",
                table: "InputOutputTestCase",
                column: "ProblemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemTags_Problems_ProblemsProblemId",
                table: "ProblemTags",
                column: "ProblemsProblemId",
                principalTable: "Problems",
                principalColumn: "ProblemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemTags_Tags_TagsTagId",
                table: "ProblemTags",
                column: "TagsTagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProblemTags_Problems_ProblemsProblemId",
                table: "ProblemTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ProblemTags_Tags_TagsTagId",
                table: "ProblemTags");

            migrationBuilder.DropTable(
                name: "InputOutputTestCase");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProblemTags",
                table: "ProblemTags");

            migrationBuilder.DropColumn(
                name: "numberOfTestCases",
                table: "Problems");

            migrationBuilder.RenameTable(
                name: "ProblemTags",
                newName: "ProblemTag");

            migrationBuilder.RenameIndex(
                name: "IX_ProblemTags_TagsTagId",
                table: "ProblemTag",
                newName: "IX_ProblemTag_TagsTagId");

            migrationBuilder.AddColumn<string>(
                name: "SampleInput",
                table: "Problems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SampleOutput",
                table: "Problems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProblemTag",
                table: "ProblemTag",
                columns: new[] { "ProblemsProblemId", "TagsTagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemTag_Problems_ProblemsProblemId",
                table: "ProblemTag",
                column: "ProblemsProblemId",
                principalTable: "Problems",
                principalColumn: "ProblemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemTag_Tags_TagsTagId",
                table: "ProblemTag",
                column: "TagsTagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
