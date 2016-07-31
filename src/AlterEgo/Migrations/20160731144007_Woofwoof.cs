using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AlterEgo.Migrations
{
    public partial class Woofwoof : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_News",
                table: "News");

            migrationBuilder.DropColumn(
                name: "NewsId",
                table: "News");

            migrationBuilder.AlterColumn<string>(
                name: "Character",
                table: "News",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_News",
                table: "News",
                columns: new[] { "Timestamp", "Character" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_News",
                table: "News");

            migrationBuilder.AddColumn<int>(
                name: "NewsId",
                table: "News",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGeneratedOnAdd", true);

            migrationBuilder.AlterColumn<string>(
                name: "Character",
                table: "News",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_News",
                table: "News",
                column: "NewsId");
        }
    }
}
