using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixReservationSeatFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EVENT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Venue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVENT", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USER",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SECTOR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SECTOR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SECTOR_EVENT_EventId",
                        column: x => x.EventId,
                        principalTable: "EVENT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_LOG",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_LOG", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AUDIT_LOG_USER_UserId",
                        column: x => x.UserId,
                        principalTable: "USER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SEAT",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectorId = table.Column<int>(type: "int", nullable: false),
                    RowIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeatNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SEAT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SEAT_SECTOR_SectorId",
                        column: x => x.SectorId,
                        principalTable: "SECTOR",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RESERVATION",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    SeatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReservedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RESERVATION", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RESERVATION_SEAT_SeatId",
                        column: x => x.SeatId,
                        principalTable: "SEAT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RESERVATION_USER_UserId",
                        column: x => x.UserId,
                        principalTable: "USER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "EVENT",
                columns: new[] { "Id", "EventDate", "Name", "Status", "Venue" },
                values: new object[] { 1, new DateTime(2026, 6, 15, 20, 0, 0, 0, DateTimeKind.Unspecified), "Concierto de Rock", "Activo", "Estadio Nacional" });

            migrationBuilder.InsertData(
                table: "SECTOR",
                columns: new[] { "Id", "Capacity", "EventId", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 50, 1, "Campo", 5000.00m },
                    { 2, 50, 1, "Platea", 8000.00m }
                });

            migrationBuilder.InsertData(
                table: "SEAT",
                columns: new[] { "Id", "RowIdentifier", "SeatNumber", "SectorId", "Status", "Version" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "A", 1, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "A", 2, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "A", 3, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "A", 4, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "A", 5, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "A", 6, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000007"), "A", 7, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000008"), "A", 8, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000009"), "A", 9, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000000a"), "A", 10, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000000b"), "A", 11, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000000c"), "A", 12, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000000d"), "A", 13, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000000e"), "A", 14, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000000f"), "A", 15, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000010"), "A", 16, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000011"), "A", 17, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000012"), "A", 18, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000013"), "A", 19, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000014"), "A", 20, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000015"), "A", 21, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000016"), "A", 22, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000017"), "A", 23, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000018"), "A", 24, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000019"), "A", 25, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000001a"), "A", 26, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000001b"), "A", 27, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000001c"), "A", 28, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000001d"), "A", 29, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000001e"), "A", 30, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000001f"), "A", 31, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000020"), "A", 32, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000021"), "A", 33, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000022"), "A", 34, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000023"), "A", 35, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000024"), "A", 36, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000025"), "A", 37, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000026"), "A", 38, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000027"), "A", 39, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000028"), "A", 40, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000029"), "A", 41, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000002a"), "A", 42, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000002b"), "A", 43, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000002c"), "A", 44, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000002d"), "A", 45, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000002e"), "A", 46, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-00000000002f"), "A", 47, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000030"), "A", 48, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000031"), "A", 49, 1, "Available", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000032"), "A", 50, 1, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000001"), "B", 1, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "B", 2, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "B", 3, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "B", 4, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000005"), "B", 5, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000006"), "B", 6, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000007"), "B", 7, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000008"), "B", 8, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000009"), "B", 9, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000000a"), "B", 10, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000000b"), "B", 11, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000000c"), "B", 12, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000000d"), "B", 13, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000000e"), "B", 14, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000000f"), "B", 15, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000010"), "B", 16, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000011"), "B", 17, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000012"), "B", 18, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000013"), "B", 19, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000014"), "B", 20, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000015"), "B", 21, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000016"), "B", 22, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000017"), "B", 23, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000018"), "B", 24, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000019"), "B", 25, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000001a"), "B", 26, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000001b"), "B", 27, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000001c"), "B", 28, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000001d"), "B", 29, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000001e"), "B", 30, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000001f"), "B", 31, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000020"), "B", 32, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000021"), "B", 33, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000022"), "B", 34, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000023"), "B", 35, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000024"), "B", 36, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000025"), "B", 37, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000026"), "B", 38, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000027"), "B", 39, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000028"), "B", 40, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000029"), "B", 41, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000002a"), "B", 42, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000002b"), "B", 43, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000002c"), "B", 44, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000002d"), "B", 45, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000002e"), "B", 46, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-00000000002f"), "B", 47, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000030"), "B", 48, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000031"), "B", 49, 2, "Available", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000032"), "B", 50, 2, "Available", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_LOG_UserId",
                table: "AUDIT_LOG",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RESERVATION_SeatId",
                table: "RESERVATION",
                column: "SeatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RESERVATION_UserId",
                table: "RESERVATION",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SEAT_SectorId",
                table: "SEAT",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_SECTOR_EventId",
                table: "SECTOR",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_LOG");

            migrationBuilder.DropTable(
                name: "RESERVATION");

            migrationBuilder.DropTable(
                name: "SEAT");

            migrationBuilder.DropTable(
                name: "USER");

            migrationBuilder.DropTable(
                name: "SECTOR");

            migrationBuilder.DropTable(
                name: "EVENT");
        }
    }
}
