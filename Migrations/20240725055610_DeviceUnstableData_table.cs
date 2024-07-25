using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agri_Smart.Migrations
{
    public partial class DeviceUnstableData_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceUnstableData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MacId = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    SoilMoistureP = table.Column<double>(type: "double precision", nullable: true),
                    SoilMoistureF = table.Column<double>(type: "double precision", nullable: true),
                    TemperatureC = table.Column<double>(type: "double precision", nullable: true),
                    TemperatureF = table.Column<double>(type: "double precision", nullable: true),
                    Humidity = table.Column<double>(type: "double precision", nullable: true),
                    Nitrogen = table.Column<double>(type: "double precision", nullable: true),
                    Potassium = table.Column<double>(type: "double precision", nullable: true),
                    Phosphorus = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceUnstableData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceUnstableData");
        }
    }
}
