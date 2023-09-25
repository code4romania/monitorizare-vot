using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingIrregularities.Domain.Seed.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentFileName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "NotesAttachments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "NotesAttachments");
        }
    }
}
