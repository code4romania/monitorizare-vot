using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VotingIrregularities.Domain.Migrations
{
    public partial class NoteAvailableForSectie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdIntrebare",
                table: "Nota",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdIntrebare",
                table: "Nota",
                nullable: false);
        }
    }
}
