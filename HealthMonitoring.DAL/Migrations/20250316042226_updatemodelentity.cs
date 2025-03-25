using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthMonitoring.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updatemodelentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sensorDataPoints_sensorDataSets_DataSetId",
                table: "sensorDataPoints");

            migrationBuilder.DropTable(
                name: "sensorDataSets");

            migrationBuilder.DropIndex(
                name: "IX_sensorDataPoints_DataSetId",
                table: "sensorDataPoints");

            migrationBuilder.DropColumn(
                name: "DataSetId",
                table: "sensorDataPoints");

            migrationBuilder.DropColumn(
                name: "SequenceNumber",
                table: "sensorDataPoints");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "bloodPressureReadings");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "sensorDataPoints",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_sensorDataPoints_UserId",
                table: "sensorDataPoints",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_sensorDataPoints_AspNetUsers_UserId",
                table: "sensorDataPoints",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sensorDataPoints_AspNetUsers_UserId",
                table: "sensorDataPoints");

            migrationBuilder.DropIndex(
                name: "IX_sensorDataPoints_UserId",
                table: "sensorDataPoints");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "sensorDataPoints");

            migrationBuilder.AddColumn<int>(
                name: "DataSetId",
                table: "sensorDataPoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SequenceNumber",
                table: "sensorDataPoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BatchId",
                table: "bloodPressureReadings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "sensorDataSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalPoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensorDataSets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sensorDataPoints_DataSetId",
                table: "sensorDataPoints",
                column: "DataSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_sensorDataPoints_sensorDataSets_DataSetId",
                table: "sensorDataPoints",
                column: "DataSetId",
                principalTable: "sensorDataSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
