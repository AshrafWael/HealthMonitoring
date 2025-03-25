using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthMonitoring.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updatedatebase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "APP",
                table: "sensorDataPoints");

            migrationBuilder.AlterColumn<double>(
                name: "PPG",
                table: "sensorDataPoints",
                type: "float",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<double>(
                name: "ECG",
                table: "sensorDataPoints",
                type: "float",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<double>(
                name: "ABP",
                table: "sensorDataPoints",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "sensorDataPoints",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<double>(
                name: "SystolicPressure",
                table: "bloodPressureReadings",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "DiastolicPressure",
                table: "bloodPressureReadings",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ABP",
                table: "sensorDataPoints");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "sensorDataPoints");

            migrationBuilder.AlterColumn<string>(
                name: "PPG",
                table: "sensorDataPoints",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "ECG",
                table: "sensorDataPoints",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "APP",
                table: "sensorDataPoints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<float>(
                name: "SystolicPressure",
                table: "bloodPressureReadings",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "DiastolicPressure",
                table: "bloodPressureReadings",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
