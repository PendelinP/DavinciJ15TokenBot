using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DavinciJ15TokenBot.DataManager.EF.Migrations
{
    public partial class UpdatedChatIdType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "TelegramChatId",
                table: "Members",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "MemberSinceUtc",
                table: "Members",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationValidSinceUtc",
                table: "Members",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationValidSinceUtc",
                table: "Members");

            migrationBuilder.AlterColumn<int>(
                name: "TelegramChatId",
                table: "Members",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "MemberSinceUtc",
                table: "Members",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
