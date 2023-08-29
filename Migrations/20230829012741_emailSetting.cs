using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bubblevel_MatchService.Migrations
{
    /// <inheritdoc />
    public partial class emailSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "EmailSetting",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailSetting",
                table: "EmailSetting",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailSetting",
                table: "EmailSetting");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EmailSetting");
        }
    }
}
