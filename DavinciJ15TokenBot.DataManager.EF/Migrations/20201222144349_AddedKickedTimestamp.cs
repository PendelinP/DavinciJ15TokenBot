using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DavinciJ15TokenBot.DataManager.EF.Migrations
{
    public partial class AddedKickedTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "KickedAtUtc",
                table: "Members",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KickedAtUtc",
                table: "Members");
        }
    }
}
