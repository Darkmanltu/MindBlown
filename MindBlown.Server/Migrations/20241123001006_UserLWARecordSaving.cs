using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindBlown.Server.Migrations
{
    /// <inheritdoc />
    public partial class UserLWARecordSaving : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LWARecordId",
                table: "UserWithMnemonicsIDs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LWARecordId",
                table: "UserWithMnemonicsIDs");
        }
    }
}
