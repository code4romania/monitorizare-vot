using Microsoft.EntityFrameworkCore.Migrations;

namespace VoteMonitor.Entities.Migrations
{
    public partial class AddedMobileDeviceIdTypeOnObserver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MobileDeviceIdType",
                table: "Observers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Observers_MobileDeviceId",
                table: "Observers",
                column: "MobileDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Observers_MobileDeviceIdType",
                table: "Observers",
                column: "MobileDeviceIdType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Observers_MobileDeviceId",
                table: "Observers");

            migrationBuilder.DropIndex(
                name: "IX_Observers_MobileDeviceIdType",
                table: "Observers");

            migrationBuilder.DropColumn(
                name: "MobileDeviceIdType",
                table: "Observers");
        }
    }
}
