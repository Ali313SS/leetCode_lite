using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJudge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationBetweenSubmissionAndGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Submission",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submission_GroupId",
                table: "Submission",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_Groups_GroupId",
                table: "Submission",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submission_Groups_GroupId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Submission_GroupId",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Submission");
        }
    }
}
