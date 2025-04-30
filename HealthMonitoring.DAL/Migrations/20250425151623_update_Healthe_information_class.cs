using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthMonitoring.DAL.Migrations
{
    /// <inheritdoc />
    public partial class update_Healthe_information_class : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Allergies",
                table: "HealthInformations");

            migrationBuilder.DropColumn(
                name: "ChronicDiseases",
                table: "HealthInformations");

            migrationBuilder.DropColumn(
                name: "Medications",
                table: "HealthInformations");

            migrationBuilder.AddColumn<DateTime>(
                name: "RecordedAt",
                table: "HealthInformations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordedAt",
                table: "HealthInformations");

            migrationBuilder.AddColumn<string>(
                name: "Allergies",
                table: "HealthInformations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChronicDiseases",
                table: "HealthInformations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Medications",
                table: "HealthInformations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
