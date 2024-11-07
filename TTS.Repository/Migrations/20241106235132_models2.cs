using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class models2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultantWorksOnProject_Consultants_ConsultantId",
                table: "ConsultantWorksOnProject");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsultantWorksOnProject_Project_ProjectId",
                table: "ConsultantWorksOnProject");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Clients_ClientId",
                table: "Project");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Project",
                table: "Project");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConsultantWorksOnProject",
                table: "ConsultantWorksOnProject");

            migrationBuilder.RenameTable(
                name: "Project",
                newName: "Projects");

            migrationBuilder.RenameTable(
                name: "ConsultantWorksOnProject",
                newName: "ConsultantWorksOnProjects");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ClientId",
                table: "Projects",
                newName: "IX_Projects_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ConsultantWorksOnProject_ProjectId",
                table: "ConsultantWorksOnProjects",
                newName: "IX_ConsultantWorksOnProjects_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ConsultantWorksOnProject_ConsultantId",
                table: "ConsultantWorksOnProjects",
                newName: "IX_ConsultantWorksOnProjects_ConsultantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Projects",
                table: "Projects",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConsultantWorksOnProjects",
                table: "ConsultantWorksOnProjects",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_Consultants_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Consultants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activities_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CommentBody = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CreatedById",
                table: "Activities",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ProjectId",
                table: "Activities",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedById",
                table: "Comments",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultantWorksOnProjects_Consultants_ConsultantId",
                table: "ConsultantWorksOnProjects",
                column: "ConsultantId",
                principalTable: "Consultants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultantWorksOnProjects_Projects_ProjectId",
                table: "ConsultantWorksOnProjects",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Clients_ClientId",
                table: "Projects",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultantWorksOnProjects_Consultants_ConsultantId",
                table: "ConsultantWorksOnProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsultantWorksOnProjects_Projects_ProjectId",
                table: "ConsultantWorksOnProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Clients_ClientId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Projects",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConsultantWorksOnProjects",
                table: "ConsultantWorksOnProjects");

            migrationBuilder.RenameTable(
                name: "Projects",
                newName: "Project");

            migrationBuilder.RenameTable(
                name: "ConsultantWorksOnProjects",
                newName: "ConsultantWorksOnProject");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_ClientId",
                table: "Project",
                newName: "IX_Project_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ConsultantWorksOnProjects_ProjectId",
                table: "ConsultantWorksOnProject",
                newName: "IX_ConsultantWorksOnProject_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ConsultantWorksOnProjects_ConsultantId",
                table: "ConsultantWorksOnProject",
                newName: "IX_ConsultantWorksOnProject_ConsultantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Project",
                table: "Project",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConsultantWorksOnProject",
                table: "ConsultantWorksOnProject",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultantWorksOnProject_Consultants_ConsultantId",
                table: "ConsultantWorksOnProject",
                column: "ConsultantId",
                principalTable: "Consultants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultantWorksOnProject_Project_ProjectId",
                table: "ConsultantWorksOnProject",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Clients_ClientId",
                table: "Project",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }
    }
}
