using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fullerenes.Server.Migrations
{
    /// <inheritdoc />
    public partial class DBNormolize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fullerenes");

            migrationBuilder.DropTable(
                name: "LimitedAreas");

            migrationBuilder.CreateTable(
                name: "sp_data",
                columns: table => new
                {
                    super_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    path_to_file = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sp_data", x => x.super_id);
                });

            migrationBuilder.CreateTable(
                name: "sp_gen",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    series = table.Column<int>(type: "integer", nullable: false),
                    phi = table.Column<float>(type: "real", nullable: true),
                    number_of_generation = table.Column<long>(type: "bigint", nullable: false),
                    area_type = table.Column<int>(type: "integer", nullable: true),
                    fullerene_type = table.Column<int>(type: "integer", nullable: true),
                    super_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sp_gen", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sp_data");

            migrationBuilder.DropTable(
                name: "sp_gen");

            migrationBuilder.CreateTable(
                name: "LimitedAreas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AreaType = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Center = table.Column<string>(type: "text", nullable: false),
                    RequestedNumberOfFullerenes = table.Column<int>(type: "integer", nullable: false),
                    Radius = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitedAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fullerenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LimitedAreaId = table.Column<int>(type: "integer", nullable: false),
                    Center = table.Column<string>(type: "text", nullable: false),
                    EulerAngles = table.Column<string>(type: "text", nullable: false),
                    FullereneType = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Series = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fullerenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fullerenes_LimitedAreas_LimitedAreaId",
                        column: x => x.LimitedAreaId,
                        principalTable: "LimitedAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fullerenes_LimitedAreaId",
                table: "Fullerenes",
                column: "LimitedAreaId");
        }
    }
}
