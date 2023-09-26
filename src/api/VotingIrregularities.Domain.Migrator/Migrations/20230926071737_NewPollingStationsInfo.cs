using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingIrregularities.Domain.Seed.Migrations
{
    /// <inheritdoc />
    public partial class NewPollingStationsInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPollingStationPresidentFemale",
                table: "PollingStationInfos");

            migrationBuilder.AddColumn<bool>(
                name: "AdequatePollingStationSize",
                table: "PollingStationInfos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ChairmanPresence",
                table: "PollingStationInfos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MinPresentMembers",
                table: "PollingStationInfos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfCommissionMembers",
                table: "PollingStationInfos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfFemaleMembers",
                table: "PollingStationInfos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfVotersOnTheList",
                table: "PollingStationInfos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SinglePollingStationOrCommission",
                table: "PollingStationInfos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdequatePollingStationSize",
                table: "PollingStationInfos");

            migrationBuilder.DropColumn(
                name: "ChairmanPresence",
                table: "PollingStationInfos");

            migrationBuilder.DropColumn(
                name: "MinPresentMembers",
                table: "PollingStationInfos");

            migrationBuilder.DropColumn(
                name: "NumberOfCommissionMembers",
                table: "PollingStationInfos");

            migrationBuilder.DropColumn(
                name: "NumberOfFemaleMembers",
                table: "PollingStationInfos");

            migrationBuilder.DropColumn(
                name: "NumberOfVotersOnTheList",
                table: "PollingStationInfos");

            migrationBuilder.DropColumn(
                name: "SinglePollingStationOrCommission",
                table: "PollingStationInfos");

            migrationBuilder.AddColumn<bool>(
                name: "IsPollingStationPresidentFemale",
                table: "PollingStationInfos",
                type: "boolean",
                nullable: true);
        }
    }
}
