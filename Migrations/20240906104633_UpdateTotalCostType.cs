using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agri_Smart.Migrations
{
    public partial class UpdateTotalCostType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use raw SQL to alter the column with explicit conversion
            migrationBuilder.Sql(
                @"ALTER TABLE ""Expenses""
              ALTER COLUMN ""TotalCost"" TYPE double precision
              USING NULLIF(""TotalCost"", '')::double precision;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Convert double precision back to text (if needed)
            migrationBuilder.Sql(
                @"ALTER TABLE ""Expenses""
              ALTER COLUMN ""TotalCost"" TYPE text
              USING ""TotalCost""::text;");
        }
    }
}
