using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agri_Smart.Migrations
{
    public partial class addingCustomerRevenueschanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "CustomerRevenue",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "CustomerRevenue",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "CustomerRevenue",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CustomerRevenue");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "CustomerRevenue");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "CustomerRevenue");
        }
    }
}
