using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace heroescupreloaded.Migrations
{
    public partial class clagecategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AddColumn<int>(
                name: "AgeCategory",
                table: "Missions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AgeCategory",
                table: "Clubs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
