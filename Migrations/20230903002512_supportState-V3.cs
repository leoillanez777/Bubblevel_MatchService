using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bubblevel_MatchService.Migrations
{
    /// <inheritdoc />
    public partial class supportStateV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Step",
                table: "Setting",
                newName: "State");

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "SupportIncident",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameReceiver",
                table: "Setting",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "SupportIncident");

            migrationBuilder.DropColumn(
                name: "NameReceiver",
                table: "Setting");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Setting",
                newName: "Step");
        }
    }
}
