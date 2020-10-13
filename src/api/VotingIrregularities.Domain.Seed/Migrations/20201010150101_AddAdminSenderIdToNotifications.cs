using Microsoft.EntityFrameworkCore.Migrations;

namespace VoteMonitor.Entities.Migrations
{
    public partial class AddAdminSenderIdToNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SenderAdminId",
                table: "Notifications",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SenderAdminId",
                table: "Notifications",
                column: "SenderAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NgoAdmin_SenderAdminId",
                table: "Notifications",
                column: "SenderAdminId",
                principalTable: "NgoAdmin",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NgoAdmin_SenderAdminId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_SenderAdminId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SenderAdminId",
                table: "Notifications");
        }
    }
}
