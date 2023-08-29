using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bubblevel_MatchService.Migrations
{
    /// <inheritdoc />
    public partial class projectMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Customer");

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Exists = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportIncident",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    TotalDebited = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportIncident", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportIncident_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupportIncident_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Intervention",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupportIncidentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intervention", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Intervention_SupportIncident_SupportIncidentId",
                        column: x => x.SupportIncidentId,
                        principalTable: "SupportIncident",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Intervention_SupportIncidentId",
                table: "Intervention",
                column: "SupportIncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportIncident_CustomerId",
                table: "SupportIncident",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportIncident_ProjectId",
                table: "SupportIncident",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Intervention");

            migrationBuilder.DropTable(
                name: "SupportIncident");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Customer",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
