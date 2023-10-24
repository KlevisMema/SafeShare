using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeShare.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class RemovedRelationBetweenUserAndExpensesBeacuseTheRelationIsAlreadyConfiguredAsManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_AspNetUsers_FromUserId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_FromUserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "FromUserId",
                table: "Expenses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromUserId",
                table: "Expenses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_FromUserId",
                table: "Expenses",
                column: "FromUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_AspNetUsers_FromUserId",
                table: "Expenses",
                column: "FromUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
