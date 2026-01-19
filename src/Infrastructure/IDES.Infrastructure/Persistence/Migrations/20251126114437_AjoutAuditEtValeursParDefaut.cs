using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IDES.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AjoutAuditEtValeursParDefaut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreePar",
                table: "TypesElements",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreation",
                table: "TypesElements",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModification",
                table: "TypesElements",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiePar",
                table: "TypesElements",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreePar",
                table: "Proprietes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreation",
                table: "Proprietes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModification",
                table: "Proprietes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiePar",
                table: "Proprietes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ValeurParDefaut",
                table: "Proprietes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreePar",
                table: "TypesElements");

            migrationBuilder.DropColumn(
                name: "DateCreation",
                table: "TypesElements");

            migrationBuilder.DropColumn(
                name: "DateModification",
                table: "TypesElements");

            migrationBuilder.DropColumn(
                name: "ModifiePar",
                table: "TypesElements");

            migrationBuilder.DropColumn(
                name: "CreePar",
                table: "Proprietes");

            migrationBuilder.DropColumn(
                name: "DateCreation",
                table: "Proprietes");

            migrationBuilder.DropColumn(
                name: "DateModification",
                table: "Proprietes");

            migrationBuilder.DropColumn(
                name: "ModifiePar",
                table: "Proprietes");

            migrationBuilder.DropColumn(
                name: "ValeurParDefaut",
                table: "Proprietes");
        }
    }
}
