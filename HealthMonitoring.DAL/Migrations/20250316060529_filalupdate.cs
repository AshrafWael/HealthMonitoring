using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthMonitoring.DAL.Migrations
{
    /// <inheritdoc />
    public partial class filalupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "APPValue",
                table: "sensorDataPoints");

            migrationBuilder.DropColumn(
                name: "ECGValue",
                table: "sensorDataPoints");

            migrationBuilder.DropColumn(
                name: "PPGValue",
                table: "sensorDataPoints");

            migrationBuilder.AddColumn<string>(
                name: "APP",
                table: "sensorDataPoints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ECG",
                table: "sensorDataPoints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PPG",
                table: "sensorDataPoints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "APP",
                table: "sensorDataPoints");

            migrationBuilder.DropColumn(
                name: "ECG",
                table: "sensorDataPoints");

            migrationBuilder.DropColumn(
                name: "PPG",
                table: "sensorDataPoints");

            migrationBuilder.AddColumn<float>(
                name: "APPValue",
                table: "sensorDataPoints",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ECGValue",
                table: "sensorDataPoints",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PPGValue",
                table: "sensorDataPoints",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
