using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AlterEgo.Migrations
{
    public partial class _201607262 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LatestPostPostId",
                table: "Forums",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forums_LatestPostPostId",
                table: "Forums",
                column: "LatestPostPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Forums_Posts_LatestPostPostId",
                table: "Forums",
                column: "LatestPostPostId",
                principalTable: "Posts",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forums_Posts_LatestPostPostId",
                table: "Forums");

            migrationBuilder.DropIndex(
                name: "IX_Forums_LatestPostPostId",
                table: "Forums");

            migrationBuilder.DropColumn(
                name: "LatestPostPostId",
                table: "Forums");
        }
    }
}
