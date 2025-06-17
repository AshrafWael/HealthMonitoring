using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthMonitoring.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updateuserandemergancycontactrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyContacts_AspNetUsers_UserId",
                table: "EmergencyContacts");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyContacts_UserId",
                table: "EmergencyContacts");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "EmergencyContacts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "ApplicationUserEmergencyContact",
                columns: table => new
                {
                    ApplicationUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmergencyContactsContactId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserEmergencyContact", x => new { x.ApplicationUsersId, x.EmergencyContactsContactId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserEmergencyContact_AspNetUsers_ApplicationUsersId",
                        column: x => x.ApplicationUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserEmergencyContact_EmergencyContacts_EmergencyContactsContactId",
                        column: x => x.EmergencyContactsContactId,
                        principalTable: "EmergencyContacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserEmergencyContact_EmergencyContactsContactId",
                table: "ApplicationUserEmergencyContact",
                column: "EmergencyContactsContactId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserEmergencyContact");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "EmergencyContacts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContacts_UserId",
                table: "EmergencyContacts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyContacts_AspNetUsers_UserId",
                table: "EmergencyContacts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
