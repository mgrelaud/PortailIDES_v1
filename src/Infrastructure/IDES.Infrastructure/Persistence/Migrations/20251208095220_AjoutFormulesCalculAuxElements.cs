using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IDES.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AjoutFormulesCalculAuxElements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormuleAcierHA",
                table: "TypesElements",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormuleAcierTS",
                table: "TypesElements",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormuleAvantMetre",
                table: "TypesElements",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormuleBeton",
                table: "TypesElements",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormuleCoffrage",
                table: "TypesElements",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormuleAcierHA",
                table: "TypesElements");

            migrationBuilder.DropColumn(
                name: "FormuleAcierTS",
                table: "TypesElements");

            migrationBuilder.DropColumn(
                name: "FormuleAvantMetre",
                table: "TypesElements");

            migrationBuilder.DropColumn(
                name: "FormuleBeton",
                table: "TypesElements");

            migrationBuilder.DropColumn(
                name: "FormuleCoffrage",
                table: "TypesElements");
        }
    }
}
