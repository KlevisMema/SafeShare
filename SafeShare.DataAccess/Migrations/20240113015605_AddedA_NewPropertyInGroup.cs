using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeShare.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddedA_NewPropertyInGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Tag",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Groups");
        }
    }
}
