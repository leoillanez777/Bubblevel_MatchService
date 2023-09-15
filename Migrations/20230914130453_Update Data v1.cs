using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bubblevel_MatchService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatav1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EmailSetting",
                columns: new[] { "Id", "Host", "Password", "Port", "UseSsl", "UseTls", "Username" },
                values: new object[] { 1, "smtp.gmail.com", "epeq bxkr wjvh lnpj", 465, true, false, "bubblevel.services@gmail.com" });

            migrationBuilder.InsertData(
                table: "Setting",
                columns: new[] { "Id", "DeliveryService", "EmailSender", "Name", "NameReceiver", "ResponseByEmail", "State", "Summary" },
                values: new object[,]
                {
                    { 1, 1, null, "Bubblevel", null, true, 0, "Confirme Service" },
                    { 2, 0, "bubblevel.services@gmail.com", "Bubblevel", null, false, 1, null },
                    { 3, 2, "bubblevel.services@gmail.com", "Finance", null, true, 2, null },
                    { 4, 2, "bubblevel.services@gmail.com", "Bubblevel", null, false, 3, null },
                    { 5, 0, null, null, null, false, 4, null },
                    { 6, 1, null, "Bubblevel", null, false, 5, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmailSetting",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Setting",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Setting",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Setting",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Setting",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Setting",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Setting",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
