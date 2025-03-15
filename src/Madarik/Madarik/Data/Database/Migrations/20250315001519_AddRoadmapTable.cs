using System;
using Madarik.Madarik.Data.Roadmap;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Madarik.Madarik.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRoadmapTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roadmap",
                schema: "SalamHack",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    FlowChart = table.Column<FlowChart>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roadmap", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Roadmap",
                schema: "SalamHack");
        }
    }
}
