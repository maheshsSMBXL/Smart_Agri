using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agri_Smart.Migrations
{
    public partial class addingCustomerRevenues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerRevenue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RevenueCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    RevenueCategoryName = table.Column<string>(type: "text", nullable: true),
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    PriceUnits = table.Column<string>(type: "text", nullable: true),
                    QuantityUnits = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Total = table.Column<decimal>(type: "numeric", nullable: true),
                    ActivityTotal = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerRevenue", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerRevenue");
        }
    }
}
