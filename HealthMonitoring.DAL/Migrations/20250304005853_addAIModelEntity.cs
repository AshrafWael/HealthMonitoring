using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthMonitoring.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addAIModelEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Distance",
                table: "ActivityDatas");

            migrationBuilder.CreateTable(
                name: "bloodPressureReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SystolicPressure = table.Column<float>(type: "real", nullable: false),
                    DiastolicPressure = table.Column<float>(type: "real", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    BatchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bloodPressureReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bloodPressureReadings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sensorDataSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalPoints = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensorDataSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sensorDataPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataSetId = table.Column<int>(type: "int", nullable: false),
                    ECGValue = table.Column<float>(type: "real", nullable: false),
                    APPValue = table.Column<float>(type: "real", nullable: false),
                    PPGValue = table.Column<float>(type: "real", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensorDataPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sensorDataPoints_sensorDataSets_DataSetId",
                        column: x => x.DataSetId,
                        principalTable: "sensorDataSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bloodPressureReadings_UserId",
                table: "bloodPressureReadings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_sensorDataPoints_DataSetId",
                table: "sensorDataPoints",
                column: "DataSetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bloodPressureReadings");

            migrationBuilder.DropTable(
                name: "sensorDataPoints");

            migrationBuilder.DropTable(
                name: "sensorDataSets");

            migrationBuilder.AddColumn<float>(
                name: "Distance",
                table: "ActivityDatas",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
