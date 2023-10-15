using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingIrregularities.Domain.Seed.Migrations
{
    /// <inheritdoc />
    public partial class AddCorruptedAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PollingStationNumber",
                table: "PollingStationInfosCorrupted",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Observers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Answers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CorruptedAnswers",
                columns: table => new
                {
                    IdObserver = table.Column<int>(type: "integer", nullable: false),
                    IdOptionToQuestion = table.Column<int>(type: "integer", nullable: false),
                    CountyCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MunicipalityCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PollingStationNumber = table.Column<int>(type: "integer", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorruptedAnswers", x => new { x.IdObserver, x.IdOptionToQuestion, x.CountyCode, x.MunicipalityCode, x.PollingStationNumber });
                    table.ForeignKey(
                        name: "FK_CorruptedAnswers_Observers_IdObserver",
                        column: x => x.IdObserver,
                        principalTable: "Observers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CorruptedAnswers_OptionsToQuestions_IdOptionToQuestion",
                        column: x => x.IdOptionToQuestion,
                        principalTable: "OptionsToQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorruptedAnswers_IdObserver",
                table: "CorruptedAnswers",
                column: "IdObserver");

            migrationBuilder.CreateIndex(
                name: "IX_CorruptedAnswers_IdObserver_CountyCode_MunicipalityCode_Pol~",
                table: "CorruptedAnswers",
                columns: new[] { "IdObserver", "CountyCode", "MunicipalityCode", "PollingStationNumber", "LastModified" });

            migrationBuilder.CreateIndex(
                name: "IX_CorruptedAnswers_IdOptionToQuestion",
                table: "CorruptedAnswers",
                column: "IdOptionToQuestion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorruptedAnswers");

            migrationBuilder.DropColumn(
                name: "PollingStationNumber",
                table: "PollingStationInfosCorrupted");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Observers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Answers",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
