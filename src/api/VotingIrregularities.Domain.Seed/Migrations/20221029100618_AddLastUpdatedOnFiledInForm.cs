using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VoteMonitor.Entities.Migrations
{
    public partial class AddLastUpdatedOnFiledInForm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedOn",
                table: "Forms",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ObserverPhoneNumber",
                table: "AnswerQueryInfos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedOn",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "ObserverPhoneNumber",
                table: "AnswerQueryInfos");
        }
    }
}
