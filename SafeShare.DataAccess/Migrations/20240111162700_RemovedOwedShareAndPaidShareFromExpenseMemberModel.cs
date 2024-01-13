﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeShare.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class RemovedOwedShareAndPaidShareFromExpenseMemberModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwedShare",
                table: "ExpenseMembers");

            migrationBuilder.DropColumn(
                name: "PaidShare",
                table: "ExpenseMembers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OwedShare",
                table: "ExpenseMembers",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaidShare",
                table: "ExpenseMembers",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
