using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace xjtf.d.ui._2.Migrations
{
    /// <inheritdoc />
    public partial class PinnedServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PinnedServices",
                columns: table => new
                {
                    ServiceName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinnedServices", x => x.ServiceName);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PinnedServices");
        }
    }
}
