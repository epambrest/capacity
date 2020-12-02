using Microsoft.EntityFrameworkCore.Migrations;

namespace Teams.Data.Migrations
{
    public partial class AddedStatusToSprint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sprint_Task_TaskId",
                table: "Sprint");

            migrationBuilder.DropIndex(
                name: "IX_Sprint_TaskId",
                table: "Sprint");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Sprint");

            migrationBuilder.DropColumn(
                name: "StorePointInHours",
                table: "Sprint");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Sprint");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Sprint",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoryPointInHours",
                table: "Sprint",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Sprint");

            migrationBuilder.DropColumn(
                name: "StoryPointInHours",
                table: "Sprint");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Sprint",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StorePointInHours",
                table: "Sprint",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "Sprint",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sprint_TaskId",
                table: "Sprint",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sprint_Task_TaskId",
                table: "Sprint",
                column: "TaskId",
                principalTable: "Task",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
