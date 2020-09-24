using Microsoft.EntityFrameworkCore.Migrations;

namespace DavinciJ15TokenBot.DataManager.EF.Migrations
{
    public partial class AddedUniqueConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Members",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_Address",
                table: "Members",
                column: "Address",
                unique: true,
                filter: "[Address] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Members_TelegramChatId",
                table: "Members",
                column: "TelegramChatId",
                unique: true,
                filter: "[TelegramChatId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Members_TelegramId",
                table: "Members",
                column: "TelegramId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_Address",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_TelegramChatId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_TelegramId",
                table: "Members");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
