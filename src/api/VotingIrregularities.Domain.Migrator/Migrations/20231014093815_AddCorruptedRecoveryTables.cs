using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VotingIrregularities.Domain.Seed.Migrations
{
    /// <inheritdoc />
    public partial class AddCorruptedRecoveryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotesCorrupted",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdQuestion = table.Column<int>(type: "integer", nullable: true),
                    IdObserver = table.Column<int>(type: "integer", nullable: false),
                    CountyCode = table.Column<string>(type: "text", nullable: true),
                    MunicipalityCode = table.Column<string>(type: "text", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: true),
                    PollingStationId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotesCorrupted", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotesCorrupted_Observers_IdObserver",
                        column: x => x.IdObserver,
                        principalTable: "Observers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotesCorrupted_PollingStations_PollingStationId",
                        column: x => x.PollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotesCorrupted_Questions_IdQuestion",
                        column: x => x.IdQuestion,
                        principalTable: "Questions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PollingStationInfosCorrupted",
                columns: table => new
                {
                    IdObserver = table.Column<int>(type: "integer", nullable: false),
                    CountyCode = table.Column<string>(type: "text", nullable: false),
                    MunicipalityCode = table.Column<string>(type: "text", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    ObserverArrivalTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ObserverLeaveTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NumberOfVotersOnTheList = table.Column<int>(type: "integer", nullable: false),
                    NumberOfCommissionMembers = table.Column<int>(type: "integer", nullable: false),
                    NumberOfFemaleMembers = table.Column<int>(type: "integer", nullable: false),
                    MinPresentMembers = table.Column<int>(type: "integer", nullable: false),
                    ChairmanPresence = table.Column<bool>(type: "boolean", nullable: false),
                    SinglePollingStationOrCommission = table.Column<bool>(type: "boolean", nullable: false),
                    AdequatePollingStationSize = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationInfosCorrupted", x => new { x.IdObserver, x.CountyCode, x.MunicipalityCode });
                    table.ForeignKey(
                        name: "FK_PollingStationInfosCorrupted_Observers_IdObserver",
                        column: x => x.IdObserver,
                        principalTable: "Observers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotesAttachmentCorrupted",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NoteId = table.Column<int>(type: "integer", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: true),
                    FileName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotesAttachmentCorrupted", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotesAttachmentCorrupted_NotesCorrupted_NoteId",
                        column: x => x.NoteId,
                        principalTable: "NotesCorrupted",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotesAttachmentCorrupted_NoteId",
                table: "NotesAttachmentCorrupted",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_NotesCorrupted_CountyCode",
                table: "NotesCorrupted",
                column: "CountyCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotesCorrupted_IdObserver",
                table: "NotesCorrupted",
                column: "IdObserver");

            migrationBuilder.CreateIndex(
                name: "IX_NotesCorrupted_IdQuestion",
                table: "NotesCorrupted",
                column: "IdQuestion");

            migrationBuilder.CreateIndex(
                name: "IX_NotesCorrupted_MunicipalityCode",
                table: "NotesCorrupted",
                column: "MunicipalityCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotesCorrupted_PollingStationId",
                table: "NotesCorrupted",
                column: "PollingStationId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInfosCorrupted_IdObserver",
                table: "PollingStationInfosCorrupted",
                column: "IdObserver");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotesAttachmentCorrupted");

            migrationBuilder.DropTable(
                name: "PollingStationInfosCorrupted");

            migrationBuilder.DropTable(
                name: "NotesCorrupted");
        }
    }
}
