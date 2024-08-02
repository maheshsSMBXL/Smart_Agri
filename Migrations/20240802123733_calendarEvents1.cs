using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agri_Smart.Migrations
{
    public partial class calendarEvents1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CalendarEvents",
                table: "CalendarEvents");

            migrationBuilder.RenameTable(
                name: "CalendarEvents",
                newName: "CalendarCommonEvents");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CalendarCommonEvents",
                table: "CalendarCommonEvents",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CalendarCommonEvents",
                table: "CalendarCommonEvents");

            migrationBuilder.RenameTable(
                name: "CalendarCommonEvents",
                newName: "CalendarEvents");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CalendarEvents",
                table: "CalendarEvents",
                column: "Id");
        }
    }
}
