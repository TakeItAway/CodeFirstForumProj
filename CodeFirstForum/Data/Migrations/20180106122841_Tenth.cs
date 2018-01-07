using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CodeFirstForum.Data.Migrations
{
    public partial class Tenth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NextId",
                table: "Steps",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrevId",
                table: "Steps",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextId",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "PrevId",
                table: "Steps");
        }
    }
}
