using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CodeFirstForum.Data.Migrations
{
    public partial class Sixth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InstructionId",
                table: "ManualTags",
                newName: "ManualId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ManualId",
                table: "ManualTags",
                newName: "InstructionId");
        }
    }
}
