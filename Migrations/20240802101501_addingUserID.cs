using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agri_Smart.Migrations
{
    public partial class addingUserID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Workers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "OtherExpenses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Machineries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "CategorySubExpenses",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OtherExpenses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Machineries");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CategorySubExpenses");
        }
    }
}
