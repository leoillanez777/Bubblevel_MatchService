using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bubblevel_MatchService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjecModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Closed",
                table: "Project",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Hours",
                table: "Project",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IntialDate",
                table: "Project",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InterventionDate",
                table: "Intervention",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Closed",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Hours",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "IntialDate",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "InterventionDate",
                table: "Intervention");
        }
    }
}
