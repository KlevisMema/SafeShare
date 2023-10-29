using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeShare.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class RemovedTheCustomLockoutLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLockoutDateTime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LockoutCount",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastLockoutDateTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LockoutCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
