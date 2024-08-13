using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agri_Smart.Migrations
{
    public partial class AgronomicDetailNewColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoffeeType",
                table: "AgronomicDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlantingPhase",
                table: "AgronomicDetail",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoffeeType",
                table: "AgronomicDetail");

            migrationBuilder.DropColumn(
                name: "PlantingPhase",
                table: "AgronomicDetail");
        }
    }
}
