using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bubblevel_MatchService.Migrations
{
    /// <inheritdoc />
    public partial class addComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncidentNumber",
                table: "SupportIncident");

            migrationBuilder.DropColumn(
                name: "Exists",
                table: "Project");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDebited",
                table: "SupportIncident",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Project",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Duration",
                table: "Intervention",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupportIncidentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_SupportIncident_SupportIncidentId",
                        column: x => x.SupportIncidentId,
                        principalTable: "SupportIncident",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailSetting",
                columns: table => new
                {
                    Host = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UseSsl = table.Column<bool>(type: "bit", nullable: false),
                    UseTls = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_SupportIncidentId",
                table: "Comment",
                column: "SupportIncidentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "EmailSetting");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Intervention");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDebited",
                table: "SupportIncident",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncidentNumber",
                table: "SupportIncident",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Exists",
                table: "Project",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
