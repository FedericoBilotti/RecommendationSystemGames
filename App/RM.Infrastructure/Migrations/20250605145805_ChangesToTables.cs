using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangesToTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genre_Game_GenreId",
                table: "Genre");

            migrationBuilder.AddColumn<Guid>(
                name: "GameId",
                table: "Genre",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genre_GameId",
                table: "Genre",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Genre_Game_GameId",
                table: "Genre",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genre_Game_GameId",
                table: "Genre");

            migrationBuilder.DropIndex(
                name: "IX_Genre_GameId",
                table: "Genre");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Genre");

            migrationBuilder.AddForeignKey(
                name: "FK_Genre_Game_GenreId",
                table: "Genre",
                column: "GenreId",
                principalTable: "Game",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
