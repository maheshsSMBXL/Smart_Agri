using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agri_Smart.Migrations
{
    public partial class sensorAverageData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SensorAverageData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    window_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    window_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    macAddress = table.Column<string>(type: "text", nullable: true),
                    avg_temperature = table.Column<double>(type: "double precision", nullable: true),
                    avg_temperatureF = table.Column<double>(type: "double precision", nullable: true),
                    avg_humidity = table.Column<double>(type: "double precision", nullable: true),
                    avg_soilMoistureValue = table.Column<double>(type: "double precision", nullable: true),
                    avg_soilMoisturePercent = table.Column<double>(type: "double precision", nullable: true),
                    avg_nitrogen = table.Column<double>(type: "double precision", nullable: true),
                    avg_phosphorous = table.Column<double>(type: "double precision", nullable: true),
                    avg_potassium = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorAverageData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SensorAverageData");
        }
    }
}
