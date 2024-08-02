using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agri_Smart.Migrations
{
    public partial class addingRevenue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Revenue",
                columns: table => new
                {
                    RevenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    RevenueName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Revenue", x => x.RevenueId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Revenue");
        }
    }
}
