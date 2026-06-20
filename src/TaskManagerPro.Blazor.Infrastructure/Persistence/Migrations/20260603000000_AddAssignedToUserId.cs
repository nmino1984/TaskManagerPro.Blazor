using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Context;

#nullable disable

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Migrations
{
    // [DbContext] normally lives in the auto-generated *.Designer.cs companion file.
    // This migration never had one (hand-written, not via "dotnet ef migrations add"),
    // so EF couldn't associate it with ApplicationDbContext and silently skipped it
    // during Migrate() — added here directly to fix that.
    [DbContext(typeof(ApplicationDbContext))]
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
