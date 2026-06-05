using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Migrations
{
    [Migration("20260603000000_AddAssignedToUserId")]
    public partial class AddAssignedToUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedToUserId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToUserId",
                table: "Tasks",
                column: "AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AppUsers_AssignedToUserId",
                table: "Tasks",
                column: "AssignedToUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AppUsers_AssignedToUserId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignedToUserId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "Tasks");
        }
    }
}
