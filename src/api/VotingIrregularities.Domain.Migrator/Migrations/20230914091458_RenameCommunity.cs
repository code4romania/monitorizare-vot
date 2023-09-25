using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VotingIrregularities.Domain.Seed.Migrations
{
    /// <inheritdoc />
    public partial class RenameCommunity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PollingStations_Communities_CommunityId",
                table: "PollingStations");

            migrationBuilder.DropTable(
                name: "Communities");

            migrationBuilder.DropColumn(
                name: "UrbanArea",
                table: "PollingStationInfos");

            migrationBuilder.RenameColumn(
                name: "CommunityId",
                table: "PollingStations",
                newName: "MunicipalityId");

            migrationBuilder.RenameIndex(
                name: "IX_PollingStations_CommunityId_Number",
                table: "PollingStations",
                newName: "IX_PollingStations_MunicipalityId_Number");

            migrationBuilder.CreateTable(
                name: "Municipalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CountyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipalities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Municipalities_Counties_CountyId",
                        column: x => x.CountyId,
                        principalTable: "Counties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Municipalities_CountyId",
                table: "Municipalities",
                column: "CountyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStations_Municipalities_MunicipalityId",
                table: "PollingStations",
                column: "MunicipalityId",
                principalTable: "Municipalities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PollingStations_Municipalities_MunicipalityId",
                table: "PollingStations");

            migrationBuilder.DropTable(
                name: "Municipalities");

            migrationBuilder.RenameColumn(
                name: "MunicipalityId",
                table: "PollingStations",
                newName: "CommunityId");

            migrationBuilder.RenameIndex(
                name: "IX_PollingStations_MunicipalityId_Number",
                table: "PollingStations",
                newName: "IX_PollingStations_CommunityId_Number");

            migrationBuilder.AddColumn<bool>(
                name: "UrbanArea",
                table: "PollingStationInfos",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Communities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CountyId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Communities_Counties_CountyId",
                        column: x => x.CountyId,
                        principalTable: "Counties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Communities_CountyId",
                table: "Communities",
                column: "CountyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStations_Communities_CommunityId",
                table: "PollingStations",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
