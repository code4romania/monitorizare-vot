using Microsoft.EntityFrameworkCore.Migrations;

namespace VoteMonitor.Entities.Migrations
{
    public partial class AddIdxForDataExport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Answer_IdObserver_CountyCode_PollingStationNumber_LastModified",
                table: "Answers",
                columns: new[] { "IdObserver", "CountyCode", "PollingStationNumber", "LastModified" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Answer_IdObserver_CountyCode_PollingStationNumber_LastModified",
                table: "Answers");
        }
    }
}
