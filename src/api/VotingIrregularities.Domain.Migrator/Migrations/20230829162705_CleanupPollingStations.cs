using Microsoft.EntityFrameworkCore.Migrations;

namespace VotingIrregularities.Domain.Seed.Migrations
{
    public partial class CleanupPollingStations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdministrativeTerritoryCode",
                table: "PollingStations");

            migrationBuilder.DropColumn(
                name: "Coordinates",
                table: "PollingStations");

            migrationBuilder.DropColumn(
                name: "TerritoryCode",
                table: "PollingStations");

            migrationBuilder.DropColumn(
                name: "NumberOfPollingStations",
                table: "Counties");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdministrativeTerritoryCode",
                table: "PollingStations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Coordinates",
                table: "PollingStations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TerritoryCode",
                table: "PollingStations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPollingStations",
                table: "Counties",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
