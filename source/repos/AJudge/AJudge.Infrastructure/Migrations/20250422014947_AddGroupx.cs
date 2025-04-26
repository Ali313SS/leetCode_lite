using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJudge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProfilePicture",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Contests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RequestsTojoinGroup",
                columns: table => new
                {
                    RequestsTojoinGroupGroupId = table.Column<int>(type: "int", nullable: false),
                    RequestsTojoinGroupUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestsTojoinGroup", x => new { x.RequestsTojoinGroupGroupId, x.RequestsTojoinGroupUserId });
                    table.ForeignKey(
                        name: "FK_RequestsTojoinGroup_Groups_RequestsTojoinGroupGroupId",
                        column: x => x.RequestsTojoinGroupGroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestsTojoinGroup_Users_RequestsTojoinGroupUserId",
                        column: x => x.RequestsTojoinGroupUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestsTojoinGroup_RequestsTojoinGroupUserId",
                table: "RequestsTojoinGroup",
                column: "RequestsTojoinGroupUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestsTojoinGroup");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Contests");

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePicture",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
