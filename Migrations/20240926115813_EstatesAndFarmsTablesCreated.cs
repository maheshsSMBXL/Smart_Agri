using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agri_Smart.Migrations
{
    public partial class EstatesAndFarmsTablesCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estates",
                columns: table => new
                {
                    EstateId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstateName = table.Column<string>(type: "text", nullable: true),
                    EstateManagerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estates", x => x.EstateId);
                });

            migrationBuilder.CreateTable(
                name: "Farms",
                columns: table => new
                {
                    FarmId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstateId = table.Column<Guid>(type: "uuid", nullable: true),
                    FarmName = table.Column<string>(type: "text", nullable: true),
                    LandSize = table.Column<double>(type: "double precision", nullable: true),
                    CropGrowingStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SoilType = table.Column<string>(type: "text", nullable: true),
                    CropType = table.Column<string>(type: "text", nullable: true),
                    CropVariety = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    ZipCode = table.Column<string>(type: "text", nullable: true),
                    FarmAddress = table.Column<string>(type: "text", nullable: true),
                    BudgetAmount = table.Column<double>(type: "double precision", nullable: true),
                    DeviceStatus = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farms", x => x.FarmId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Estates");

            migrationBuilder.DropTable(
                name: "Farms");
        }
    }
}
