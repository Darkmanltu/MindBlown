using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindBlown.Server.Migrations
{
    /// <inheritdoc />
    public partial class CreateAnswerSessionsAndMnemonics2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnswerSessions",
                columns: table => new
                {
                    AnswerSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastAnswerTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CorrectCount = table.Column<int>(type: "int", nullable: false),
                    IncorrectCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerSessions", x => x.AnswerSessionId);
                });

            migrationBuilder.CreateTable(
                name: "AnsweredMnemonics",
                columns: table => new
                {
                    AnsweredMnemonicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnswerSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MnemonicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnsweredMnemonics", x => x.AnsweredMnemonicId);
                    table.ForeignKey(
                        name: "FK_AnsweredMnemonics_AnswerSessions_AnswerSessionId",
                        column: x => x.AnswerSessionId,
                        principalTable: "AnswerSessions",
                        principalColumn: "AnswerSessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnsweredMnemonics_AnswerSessionId",
                table: "AnsweredMnemonics",
                column: "AnswerSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnsweredMnemonics");

            migrationBuilder.DropTable(
                name: "AnswerSessions");
        }
    }
}
