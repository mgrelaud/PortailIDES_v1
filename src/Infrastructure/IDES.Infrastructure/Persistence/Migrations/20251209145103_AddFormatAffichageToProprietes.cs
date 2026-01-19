using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IDES.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFormatAffichageToProprietes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormatAffichage",
                table: "Proprietes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormatAffichage",
                table: "Proprietes");
        }
    }
}
