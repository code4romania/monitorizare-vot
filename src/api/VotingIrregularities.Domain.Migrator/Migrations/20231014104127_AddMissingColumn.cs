using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingIrregularities.Domain.Seed.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotesCorrupted_PollingStations_PollingStationId",
                table: "NotesCorrupted");

            migrationBuilder.DropIndex(
                name: "IX_NotesCorrupted_PollingStationId",
                table: "NotesCorrupted");

            migrationBuilder.DropColumn(
                name: "PollingStationId",
                table: "NotesCorrupted");

            migrationBuilder.AddColumn<int>(
                name: "PollingStationNumber",
                table: "NotesCorrupted",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PollingStationNumber",
                table: "NotesCorrupted");

            migrationBuilder.AddColumn<int>(
                name: "PollingStationId",
                table: "NotesCorrupted",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotesCorrupted_PollingStationId",
                table: "NotesCorrupted",
                column: "PollingStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotesCorrupted_PollingStations_PollingStationId",
                table: "NotesCorrupted",
                column: "PollingStationId",
                principalTable: "PollingStations",
                principalColumn: "Id");
        }
    }
}
