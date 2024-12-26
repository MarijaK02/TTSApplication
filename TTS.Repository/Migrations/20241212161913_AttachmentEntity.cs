using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AttachmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_ConsultantWorksOnProjects_ResponsibleConsultantId",
                table: "Activities");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResponsibleConsultantId",
                table: "Activities",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ActivityId",
                table: "Attachments",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_ConsultantWorksOnProjects_ResponsibleConsultantId",
                table: "Activities",
                column: "ResponsibleConsultantId",
                principalTable: "ConsultantWorksOnProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_ConsultantWorksOnProjects_ResponsibleConsultantId",
                table: "Activities");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResponsibleConsultantId",
                table: "Activities",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_ConsultantWorksOnProjects_ResponsibleConsultantId",
                table: "Activities",
                column: "ResponsibleConsultantId",
                principalTable: "ConsultantWorksOnProjects",
                principalColumn: "Id");
        }
    }
}
