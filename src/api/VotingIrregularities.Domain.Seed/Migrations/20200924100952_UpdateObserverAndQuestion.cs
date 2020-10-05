using Microsoft.EntityFrameworkCore.Migrations;

namespace VoteMonitor.Entities.Migrations
{
    public partial class UpdateObserverAndQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Questions",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<bool>(
                name: "IsTestObserver",
                table: "Observers",
                nullable: false,
                defaultValueSql: "0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTestObserver",
                table: "Observers");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Questions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 1000);
        }
    }
}
