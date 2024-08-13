using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agri_Smart.Migrations
{
    public partial class SgronomicPractices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgronomicPractice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<List<string>>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgronomicPractice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgronomicDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DetailType = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<List<string>>(type: "text[]", nullable: true),
                    AgronomicPracticeId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgronomicDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgronomicDetail_AgronomicPractice_AgronomicPracticeId",
                        column: x => x.AgronomicPracticeId,
                        principalTable: "AgronomicPractice",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgronomicDetail_AgronomicPracticeId",
                table: "AgronomicDetail",
                column: "AgronomicPracticeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgronomicDetail");

            migrationBuilder.DropTable(
                name: "AgronomicPractice");
        }
    }
}
