using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApuracaoPontoSimples.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeCardPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "TimeCards",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "TimeCards",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "TimeCards");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "TimeCards");
        }
    }
}
