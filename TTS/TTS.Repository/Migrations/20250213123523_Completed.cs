using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Completed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedOn",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedOn",
                table: "Activities",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedOn",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CompletedOn",
                table: "Activities");
        }
    }
}
