using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParacticeAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedparacticeApidb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoAuthor",
                table: "VideoDetails");

            migrationBuilder.DropColumn(
                name: "VideoDescription",
                table: "VideoDetails");

            migrationBuilder.DropColumn(
                name: "VideoPublishedAt",
                table: "VideoDetails");

            migrationBuilder.DropColumn(
                name: "VideoThumbnailUrl",
                table: "VideoDetails");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "VideoDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "VideoDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "VideoDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoAuthor",
                table: "VideoDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VideoDescription",
                table: "VideoDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "VideoPublishedAt",
                table: "VideoDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "VideoThumbnailUrl",
                table: "VideoDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "VideoDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
