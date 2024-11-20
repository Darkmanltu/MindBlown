using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindBlown.Server.Migrations
{
    /// <inheritdoc />
    public partial class LWARecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "sessionId",
                table: "ActiveUserSession",
                newName: "SessionId");

            migrationBuilder.RenameColumn(
                name: "lastActive",
                table: "ActiveUserSession",
                newName: "LastActive");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "ActiveUserSession",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "ActiveUserSession",
                newName: "UserId");

            migrationBuilder.CreateTable(
                name: "Record",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    helperText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    mnemonicText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    wrongTextMnemonic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Record", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Record");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "ActiveUserSession",
                newName: "sessionId");

            migrationBuilder.RenameColumn(
                name: "LastActive",
                table: "ActiveUserSession",
                newName: "lastActive");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "ActiveUserSession",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ActiveUserSession",
                newName: "userId");
        }
    }
}
