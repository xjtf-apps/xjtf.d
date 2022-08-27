using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace xjtf.d.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceEntries",
                columns: table => new
                {
                    ServiceEntryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServiceName = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ExecutablePath = table.Column<string>(type: "TEXT", nullable: false),
                    Arguments = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEntries", x => x.ServiceEntryId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordSalt = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceEntryActuals",
                columns: table => new
                {
                    ServiceEntryActualId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServiceEntryId = table.Column<int>(type: "INTEGER", nullable: false),
                    ObservedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEntryActuals", x => x.ServiceEntryActualId);
                    table.ForeignKey(
                        name: "FK_ServiceEntryActuals_ServiceEntries_ServiceEntryId",
                        column: x => x.ServiceEntryId,
                        principalTable: "ServiceEntries",
                        principalColumn: "ServiceEntryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleName = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_Roles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "ServiceEntryStates",
                columns: table => new
                {
                    ServiceEntryStateId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServiceEntryId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChangedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    DaemonManaged = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEntryStates", x => x.ServiceEntryStateId);
                    table.ForeignKey(
                        name: "FK_ServiceEntryStates_ServiceEntries_ServiceEntryId",
                        column: x => x.ServiceEntryId,
                        principalTable: "ServiceEntries",
                        principalColumn: "ServiceEntryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceEntryStates_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_UserId",
                table: "Roles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEntryActuals_ServiceEntryId",
                table: "ServiceEntryActuals",
                column: "ServiceEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEntryStates_ChangedByUserId",
                table: "ServiceEntryStates",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEntryStates_ServiceEntryId",
                table: "ServiceEntryStates",
                column: "ServiceEntryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "ServiceEntryActuals");

            migrationBuilder.DropTable(
                name: "ServiceEntryStates");

            migrationBuilder.DropTable(
                name: "ServiceEntries");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
