using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeShare.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedTheRefreshTokenModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HashedToken",
                table: "RefreshTokens",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashedToken",
                table: "RefreshTokens");
        }
    }
}
