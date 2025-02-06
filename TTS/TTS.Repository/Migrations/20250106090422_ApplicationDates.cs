using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "ConsultantProjects",
                newName: "DateApplied");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "ConsultantProjects",
                newName: "DateModified");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateModified",
                table: "ConsultantProjects",
                newName: "EndTime");

            migrationBuilder.RenameColumn(
                name: "DateApplied",
                table: "ConsultantProjects",
                newName: "StartTime");
        }
    }
}
