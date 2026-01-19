using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IDES.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AjoutTableElementPropriete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TypeElement_Propriete");

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

            migrationBuilder.CreateTable(
                name: "ElementProprietes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DefinitionElementId = table.Column<int>(type: "INTEGER", nullable: false),
                    DefinitionProprieteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Valeur = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementProprietes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElementProprietes_Proprietes_DefinitionProprieteId",
                        column: x => x.DefinitionProprieteId,
                        principalTable: "Proprietes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElementProprietes_TypesElements_DefinitionElementId",
                        column: x => x.DefinitionElementId,
                        principalTable: "TypesElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElementProprietes_DefinitionElementId",
                table: "ElementProprietes",
                column: "DefinitionElementId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementProprietes_DefinitionProprieteId",
                table: "ElementProprietes",
                column: "DefinitionProprieteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElementProprietes");

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

            migrationBuilder.CreateTable(
                name: "TypeElement_Propriete",
                columns: table => new
                {
                    ProprietesId = table.Column<int>(type: "INTEGER", nullable: false),
                    TypesElementsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeElement_Propriete", x => new { x.ProprietesId, x.TypesElementsId });
                    table.ForeignKey(
                        name: "FK_TypeElement_Propriete_Proprietes_ProprietesId",
                        column: x => x.ProprietesId,
                        principalTable: "Proprietes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TypeElement_Propriete_TypesElements_TypesElementsId",
                        column: x => x.TypesElementsId,
                        principalTable: "TypesElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TypeElement_Propriete_TypesElementsId",
                table: "TypeElement_Propriete",
                column: "TypesElementsId");
        }
    }
}
