using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bubblevel_MatchService.Migrations
{
    /// <inheritdoc />
    public partial class changeStateAndTotal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsResolved",
                table: "SupportIncident");

            migrationBuilder.RenameColumn(
                name: "TotalDebited",
                table: "SupportIncident",
                newName: "Total");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "SupportIncident",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "SupportIncident");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "SupportIncident",
                newName: "TotalDebited");

            migrationBuilder.AddColumn<bool>(
                name: "IsResolved",
                table: "SupportIncident",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
