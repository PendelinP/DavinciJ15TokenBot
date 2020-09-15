using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DavinciJ15TokenBot.DataManager.EF.Migrations
{
    public partial class AddedMemberSince : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MemberSinceUtc",
                table: "Members",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberSinceUtc",
                table: "Members");
        }
    }
}
