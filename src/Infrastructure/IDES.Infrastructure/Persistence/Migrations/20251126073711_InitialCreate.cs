using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IDES.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proprietes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    NomAffichage = table.Column<string>(type: "TEXT", nullable: false),
                    TypeDonnee = table.Column<string>(type: "TEXT", nullable: false),
                    Unite = table.Column<string>(type: "TEXT", nullable: true),
                    Categorie = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proprietes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypesElements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Categorie = table.Column<string>(type: "TEXT", nullable: false),
                    PrefixeRepere = table.Column<string>(type: "TEXT", nullable: false),
                    DesignationTemplate = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypesElements", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TypeElement_Propriete");

            migrationBuilder.DropTable(
                name: "Proprietes");

            migrationBuilder.DropTable(
                name: "TypesElements");
        }
    }
}
