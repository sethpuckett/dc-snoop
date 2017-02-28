using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dcsnoop.Migrations
{
    public partial class AddAddressStreetFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StreetName",
                table: "Addresses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetNumber",
                table: "Addresses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetQuadrant",
                table: "Addresses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetType",
                table: "Addresses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreetName",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "StreetNumber",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "StreetQuadrant",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "StreetType",
                table: "Addresses");
        }
    }
}
