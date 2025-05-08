using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJudge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addCommentTableAdAssotiateTheRelationWithBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          // migrationBuilder.CreateTable(
          //     name: "CoachRequest",
          //     columns: table => new
          //     {
          //         UserId = table.Column<int>(type: "int", nullable: false),
          //         CoachId = table.Column<int>(type: "int", nullable: false)
          //     },
          //     constraints: table =>
          //     {
          //         table.PrimaryKey("PK_CoachRequest", x => new { x.UserId, x.CoachId });
          //         table.ForeignKey(
          //             name: "FK_CoachRequest_Users_CoachId",
          //             column: x => x.CoachId,
          //             principalTable: "Users",
          //             principalColumn: "UserId",
          //             onDelete: ReferentialAction.Cascade);
          //         table.ForeignKey(
          //             name: "FK_CoachRequest_Users_UserId",
          //             column: x => x.UserId,
          //             principalTable: "Users",
          //             principalColumn: "UserId",
          //             onDelete: ReferentialAction.Restrict);
          //     });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BlogId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Blog_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blog",
                        principalColumn: "BlogId",
                        onDelete: ReferentialAction.Restrict);
                });

         //  migrationBuilder.CreateTable(
         //      name: "UserTeamInvitation",
         //      columns: table => new
         //      {
         //          UserId = table.Column<int>(type: "int", nullable: false),
         //          TeamId = table.Column<int>(type: "int", nullable: false)
         //      },
         //      constraints: table =>
         //      {
         //          table.PrimaryKey("PK_UserTeamInvitation", x => new { x.UserId, x.TeamId });
         //          table.ForeignKey(
         //              name: "FK_UserTeamInvitation_Teams_TeamId",
         //              column: x => x.TeamId,
         //              principalTable: "Teams",
         //              principalColumn: "TeamId",
         //              onDelete: ReferentialAction.Cascade);
         //          table.ForeignKey(
         //              name: "FK_UserTeamInvitation_Users_UserId",
         //              column: x => x.UserId,
         //              principalTable: "Users",
         //              principalColumn: "UserId",
         //              onDelete: ReferentialAction.Cascade);
         //      });

        //  migrationBuilder.CreateIndex(
        //      name: "IX_CoachRequest_CoachId",
        //      table: "CoachRequest",
        //      column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_BlogId",
                table: "Comment",
                column: "BlogId");

          //  migrationBuilder.CreateIndex(
          //      name: "IX_UserTeamInvitation_TeamId",
          //      table: "UserTeamInvitation",
          //      column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          //  migrationBuilder.DropTable(
          //      name: "CoachRequest");
          //
            migrationBuilder.DropTable(
                name: "Comment");

         //   migrationBuilder.DropTable(
         //       name: "UserTeamInvitation");
        }
    }
}
