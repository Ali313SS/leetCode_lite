using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJudge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class anaatbdant4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submission_Groups_GroupId",
                table: "Submission");

            migrationBuilder.DropTable(
                name: "ContestGroupMemberships");

            migrationBuilder.RenameColumn(
                name: "GroupContestId",
                table: "Contests",
                newName: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Contests_GroupId",
                table: "Contests",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contests_Groups_GroupId",
                table: "Contests",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_Groups_GroupId",
                table: "Submission",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contests_Groups_GroupId",
                table: "Contests");

            migrationBuilder.DropForeignKey(
                name: "FK_Submission_Groups_GroupId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Contests_GroupId",
                table: "Contests");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Contests",
                newName: "GroupContestId");

            migrationBuilder.CreateTable(
                name: "ContestGroupMemberships",
                columns: table => new
                {
                    ContestId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestGroupMemberships", x => new { x.ContestId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_ContestGroupMemberships_Contests_ContestId",
                        column: x => x.ContestId,
                        principalTable: "Contests",
                        principalColumn: "ContestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestGroupMemberships_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContestGroupMemberships_GroupId",
                table: "ContestGroupMemberships",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_Groups_GroupId",
                table: "Submission",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
