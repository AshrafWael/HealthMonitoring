using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthMonitoring.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Updatetablesname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sensorDataPoints");

            migrationBuilder.RenameColumn(
                name: "SystolicPressure",
                table: "bloodPressureReadings",
                newName: "sbp");

            migrationBuilder.RenameColumn(
                name: "DiastolicPressure",
                table: "bloodPressureReadings",
                newName: "dbp");

            migrationBuilder.CreateTable(
                name: "sensorDataSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ECG = table.Column<double>(type: "float", nullable: false),
                    ABP = table.Column<double>(type: "float", nullable: false),
                    PPG = table.Column<double>(type: "float", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensorDataSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sensorDataSets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sensorDataSets_UserId",
                table: "sensorDataSets",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sensorDataSets");

            migrationBuilder.RenameColumn(
                name: "sbp",
                table: "bloodPressureReadings",
                newName: "SystolicPressure");

            migrationBuilder.RenameColumn(
                name: "dbp",
                table: "bloodPressureReadings",
                newName: "DiastolicPressure");

            migrationBuilder.CreateTable(
                name: "sensorDataPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ABP = table.Column<double>(type: "float", nullable: false),
                    ECG = table.Column<double>(type: "float", nullable: false),
                    PPG = table.Column<double>(type: "float", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensorDataPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sensorDataPoints_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sensorDataPoints_UserId",
                table: "sensorDataPoints",
                column: "UserId");
        }
    }
}
