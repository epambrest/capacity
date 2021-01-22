using Microsoft.EntityFrameworkCore.Migrations;

namespace Teams.Data.Migrations
{
    public partial class MemberWorkingDays : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.CreateTable(
                name: "MemberWorkingDays",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SprintId = table.Column<int>(nullable: false),
                    MemberId = table.Column<int>(nullable: false),
                    WorkingDays = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberWorkingDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberWorkingDays_TeamMembers_MemberId",
                        column: x => x.MemberId,
                        principalTable: "TeamMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberWorkingDays_Sprint_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprint",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberWorkingDays_MemberId",
                table: "MemberWorkingDays",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberWorkingDays_SprintId",
                table: "MemberWorkingDays",
                column: "SprintId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberWorkingDays");

        }
    }
}
