using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DavinciJ15TokenBot.DataManager.PostgreSQL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TelegramId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: true),
                    LastCheckedUtc = table.Column<DateTime>(nullable: true),
                    MemberSinceUtc = table.Column<DateTime>(nullable: true),
                    RegistrationValidSinceUtc = table.Column<DateTime>(nullable: true),
                    TelegramChatId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_Address",
                table: "Members",
                column: "Address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_TelegramChatId",
                table: "Members",
                column: "TelegramChatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_TelegramId",
                table: "Members",
                column: "TelegramId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
