using Microsoft.EntityFrameworkCore.Migrations;

namespace Teams.Data.Migrations
{
    public partial class Fix_Db_Relations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_TeamMembers_MemberId",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_Sprint_SprintId",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_Team_TeamId",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Team_TeamId",
                table: "TeamMembers");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "TeamMembers",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SprintId",
                table: "Task",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "Task",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_TeamMembers_MemberId",
                table: "Task",
                column: "MemberId",
                principalTable: "TeamMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Sprint_SprintId",
                table: "Task",
                column: "SprintId",
                principalTable: "Sprint",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Team_TeamId",
                table: "Task",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Team_TeamId",
                table: "TeamMembers",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_TeamMembers_MemberId",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_Sprint_SprintId",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_Team_TeamId",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Team_TeamId",
                table: "TeamMembers");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "TeamMembers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SprintId",
                table: "Task",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "Task",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_TeamMembers_MemberId",
                table: "Task",
                column: "MemberId",
                principalTable: "TeamMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Sprint_SprintId",
                table: "Task",
                column: "SprintId",
                principalTable: "Sprint",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Team_TeamId",
                table: "Task",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Team_TeamId",
                table: "TeamMembers",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
