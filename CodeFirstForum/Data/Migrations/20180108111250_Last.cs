using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CodeFirstForum.Data.Migrations
{
    public partial class Last : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManualId",
                table: "Messanges");

            migrationBuilder.RenameColumn(
                name: "IsBlocked",
                table: "Messanges",
                newName: "IsLooked");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Messanges",
                newName: "ManualTitle");

            migrationBuilder.AddColumn<string>(
                name: "Video",
                table: "Steps",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "Messanges",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Video",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "Messanges");

            migrationBuilder.RenameColumn(
                name: "ManualTitle",
                table: "Messanges",
                newName: "AuthorId");

            migrationBuilder.RenameColumn(
                name: "IsLooked",
                table: "Messanges",
                newName: "IsBlocked");

            migrationBuilder.AddColumn<int>(
                name: "ManualId",
                table: "Messanges",
                nullable: false,
                defaultValue: 0);
        }
    }
}
