using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agri_Smart.Migrations
{
    public partial class addingCustomerRevenueschanges1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "CustomerRevenue",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "CustomerRevenue",
                type: "numeric",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "CustomerRevenue");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "CustomerRevenue");
        }
    }
}
