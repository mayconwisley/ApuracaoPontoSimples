using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApuracaoPontoSimples.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTimeCardYearMonth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month",
                table: "TimeCards");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "TimeCards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "TimeCards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "TimeCards",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
