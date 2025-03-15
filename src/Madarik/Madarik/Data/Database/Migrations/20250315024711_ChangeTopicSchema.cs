using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Madarik.Madarik.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTopicSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Topic",
                schema: "SalamHack",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Chapters = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topic", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Topic",
                schema: "SalamHack");
        }
    }
}
