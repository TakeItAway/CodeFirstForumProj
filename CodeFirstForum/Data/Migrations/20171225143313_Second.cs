using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CodeFirstForum.Data.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InstructionTags",
                table: "InstructionTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Instructions",
                table: "Instructions");

            migrationBuilder.RenameTable(
                name: "InstructionTags",
                newName: "ManualTags");

            migrationBuilder.RenameTable(
                name: "Instructions",
                newName: "Manuals");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ManualTags",
                table: "ManualTags",
                column: "ManualTagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Manuals",
                table: "Manuals",
                column: "ManualId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ManualTags",
                table: "ManualTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Manuals",
                table: "Manuals");

            migrationBuilder.RenameTable(
                name: "ManualTags",
                newName: "InstructionTags");

            migrationBuilder.RenameTable(
                name: "Manuals",
                newName: "Instructions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstructionTags",
                table: "InstructionTags",
                column: "ManualTagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Instructions",
                table: "Instructions",
                column: "ManualId");
        }
    }
}
