using Microsoft.EntityFrameworkCore.Migrations;

namespace VotingIrregularities.Domain.Migrations
{
    public partial class FormEntityAdjustments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Code",
                table: "FormVersions",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FormVersions",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "FormVersions");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FormVersions",
                newName: "Code");
        }
    }
}
