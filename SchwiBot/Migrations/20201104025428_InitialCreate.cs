using Microsoft.EntityFrameworkCore.Migrations;

namespace SchwiBot.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildStorages",
                columns: table => new
                {
                    Snowflake = table.Column<ulong>(nullable: false),
                    Storage = table.Column<string>(type: "jsonb", nullable: false),
                    UserStorage = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildStorages", x => x.Snowflake);
                });

            migrationBuilder.CreateTable(
                name: "UserStorages",
                columns: table => new
                {
                    Snowflake = table.Column<ulong>(nullable: false),
                    Storage = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStorages", x => x.Snowflake);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildStorages");

            migrationBuilder.DropTable(
                name: "UserStorages");
        }
    }
}
