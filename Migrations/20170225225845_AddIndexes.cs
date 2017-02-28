using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dcsnoop.Migrations
{
    public partial class AddIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Street",
                table: "Addresses",
                column: "Street");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_StreetName",
                table: "Addresses",
                column: "StreetName");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_StreetNumber",
                table: "Addresses",
                column: "StreetNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_StreetQuadrant",
                table: "Addresses",
                column: "StreetQuadrant");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Addresses_Street",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_StreetName",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_StreetNumber",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_StreetQuadrant",
                table: "Addresses");
        }
    }
}
