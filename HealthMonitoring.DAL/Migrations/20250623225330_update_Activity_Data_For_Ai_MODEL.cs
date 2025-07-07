using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthMonitoring.DAL.Migrations
{
    /// <inheritdoc />
    public partial class update_Activity_Data_For_Ai_MODEL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weight",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ActivityType",
                table: "ActivityDatas");

            migrationBuilder.DropColumn(
                name: "CaloriesBurned",
                table: "ActivityDatas");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "ActivityDatas");

            migrationBuilder.DropColumn(
                name: "SleepQuality",
                table: "ActivityDatas");

            migrationBuilder.RenameColumn(
                name: "RecordedAt",
                table: "ActivityDatas",
                newName: "UpdatedAt");

            migrationBuilder.AddColumn<double>(
                name: "WeightKg",
                table: "AspNetUsers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ActivityDatas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "ActivityDatas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "FairlyActiveMinutes",
                table: "ActivityDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LightlyActiveMinutes",
                table: "ActivityDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SedentaryMinutes",
                table: "ActivityDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalMinutesAsleep",
                table: "ActivityDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalSteps",
                table: "ActivityDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VeryActiveMinutes",
                table: "ActivityDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WeightKg",
                table: "ActivityDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "CaloriesPredictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PredictedCalories = table.Column<double>(type: "float", nullable: false),
                    PredictionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivityDtataId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaloriesPredictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaloriesPredictions_ActivityDatas_ActivityDtataId",
                        column: x => x.ActivityDtataId,
                        principalTable: "ActivityDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaloriesPredictions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaloriesPredictions_ActivityDtataId",
                table: "CaloriesPredictions",
                column: "ActivityDtataId");

            migrationBuilder.CreateIndex(
                name: "IX_CaloriesPredictions_UserId",
                table: "CaloriesPredictions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaloriesPredictions");

            migrationBuilder.DropColumn(
                name: "WeightKg",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ActivityDatas");

            migrationBuilder.DropColumn(
                name: "Day",
                table: "ActivityDatas");

            migrationBuilder.DropColumn(
                name: "FairlyActiveMinutes",
                table: "ActivityDatas");

            migrationBuilder.DropColumn(
                name: "LightlyActiveMinutes",
                table: "ActivityDatas");

            migrationBuilder.DropColumn(
                name: "SedentaryMinutes",
                table: "ActivityDatas");

            migrationBuilder.DropColumn(
                name: "TotalMinutesAsleep",
                table: "ActivityDatas");

            migrationBuilder.DropColumn(
                name: "TotalSteps",
                table: "ActivityDatas");

            migrationBuilder.DropColumn(
                name: "VeryActiveMinutes",
                table: "ActivityDatas");

            migrationBuilder.DropColumn(
                name: "WeightKg",
                table: "ActivityDatas");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ActivityDatas",
                newName: "RecordedAt");

            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActivityType",
                table: "ActivityDatas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CaloriesBurned",
                table: "ActivityDatas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Duration",
                table: "ActivityDatas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SleepQuality",
                table: "ActivityDatas",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
