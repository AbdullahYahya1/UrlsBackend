using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlsBackend.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateDatabase3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClicksCounter",
                table: "Urls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExpairDate",
                table: "Urls",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxClicks",
                table: "Urls",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Urls",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClicksCounter",
                table: "Urls");

            migrationBuilder.DropColumn(
                name: "ExpairDate",
                table: "Urls");

            migrationBuilder.DropColumn(
                name: "MaxClicks",
                table: "Urls");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Urls");
        }
    }
}
