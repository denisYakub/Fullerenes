using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fullerenes.Server.Migrations
{
    /// <inheritdoc />
    public partial class UserAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "fullerene_type",
                table: "sp_gen",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "area_type",
                table: "sp_gen",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "sp_veiw_group",
                columns: table => new
                {
                    sp_gen_id = table.Column<long>(type: "bigint", nullable: false),
                    count_of_generation = table.Column<long>(type: "bigint", nullable: false),
                    avg_phi = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sp_veiw_group");

            migrationBuilder.AlterColumn<int>(
                name: "fullerene_type",
                table: "sp_gen",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "area_type",
                table: "sp_gen",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
