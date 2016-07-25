using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AlterEgo.Migrations
{
    public partial class _201607261 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AuthorUserId",
                table: "Threads",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorUserId",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AuthorUserId",
                table: "Threads",
                nullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "AuthorUserId",
                table: "Posts",
                nullable: false);
        }
    }
}
