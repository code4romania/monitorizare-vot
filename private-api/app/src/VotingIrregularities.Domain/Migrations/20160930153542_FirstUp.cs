using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace VotingIrregularities.Domain.Migrations
{
    public partial class FirstUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Judet",
                columns: table => new
                {
                    IdJudet = table.Column<int>(nullable: false),
                    CodJudet = table.Column<string>(maxLength: 4, nullable: false),
                    Nume = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Judet", x => x.IdJudet);
                });

            migrationBuilder.CreateTable(
                name: "ONG",
                columns: table => new
                {
                    IdOng = table.Column<int>(nullable: false),
                    AbreviereNumeOng = table.Column<string>(maxLength: 10, nullable: false),
                    NumeONG = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ONG", x => x.IdOng);
                });

            migrationBuilder.CreateTable(
                name: "Optiune",
                columns: table => new
                {
                    IdOptiune = table.Column<int>(nullable: false),
                    SeIntroduceText = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    TextOptiune = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Optiune", x => x.IdOptiune);
                });

            migrationBuilder.CreateTable(
                name: "Sectiune",
                columns: table => new
                {
                    IdSectiune = table.Column<int>(nullable: false),
                    CodSectiune = table.Column<string>(maxLength: 4, nullable: false),
                    Descriere = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectiune", x => x.IdSectiune);
                });

            migrationBuilder.CreateTable(
                name: "VersiuneFormular",
                columns: table => new
                {
                    CodFormular = table.Column<string>(maxLength: 2, nullable: false),
                    VersiuneaCurenta = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VersiuneFormular", x => x.CodFormular);
                });

            migrationBuilder.CreateTable(
                name: "SectieDeVotare",
                columns: table => new
                {
                    IdSectieDeVotarre = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdresaSectie = table.Column<string>(maxLength: 500, nullable: true),
                    Coordonate = table.Column<string>(type: "varchar(200)", nullable: true),
                    DenumireUat = table.Column<string>(maxLength: 100, nullable: true),
                    IdJudet = table.Column<int>(nullable: false),
                    LocalitateComponenta = table.Column<string>(maxLength: 100, nullable: false),
                    NumarSectie = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectieDeVotare", x => x.IdSectieDeVotarre);
                    table.ForeignKey(
                        name: "FK_SectieDeVotare_Judet",
                        column: x => x.IdJudet,
                        principalTable: "Judet",
                        principalColumn: "IdJudet",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Observator",
                columns: table => new
                {
                    IdObservator = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EsteDinEchipa = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    IdOng = table.Column<int>(nullable: false),
                    NumarTelefon = table.Column<string>(maxLength: 20, nullable: false),
                    NumeIntreg = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observator", x => x.IdObservator);
                    table.ForeignKey(
                        name: "FK_Observator_ONG_IdOng",
                        column: x => x.IdOng,
                        principalTable: "ONG",
                        principalColumn: "IdOng",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Intrebare",
                columns: table => new
                {
                    IdIntrebare = table.Column<int>(nullable: false),
                    CodFormular = table.Column<string>(maxLength: 2, nullable: false),
                    IdSectiune = table.Column<int>(nullable: false),
                    IdTipIntrebare = table.Column<int>(nullable: false),
                    TextIntrebare = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intrebare", x => x.IdIntrebare);
                    table.ForeignKey(
                        name: "FK_Intrebare_Sectiune_IdSectiune",
                        column: x => x.IdSectiune,
                        principalTable: "Sectiune",
                        principalColumn: "IdSectiune",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccesObservatoriPerDevice",
                columns: table => new
                {
                    IdObservator = table.Column<int>(nullable: false),
                    IdDispozitivMobil = table.Column<string>(type: "varchar(500)", nullable: false),
                    DataInregistrariiDispozitivului = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccesObservatoriPerDevice", x => new { x.IdObservator, x.IdDispozitivMobil });
                    table.ForeignKey(
                        name: "FK_AccesObservatoriPerDevice_Observator_IdObservator",
                        column: x => x.IdObservator,
                        principalTable: "Observator",
                        principalColumn: "IdObservator",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DispozitivObservator",
                columns: table => new
                {
                    IdDispozitivObservator = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdObservator = table.Column<int>(nullable: false),
                    IdentificatorUnic = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispozitivObservator", x => x.IdDispozitivObservator);
                    table.ForeignKey(
                        name: "FK_DispozitivObservator_Observator_IdObservator",
                        column: x => x.IdObservator,
                        principalTable: "Observator",
                        principalColumn: "IdObservator",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RaspunsFormular",
                columns: table => new
                {
                    IdObservator = table.Column<int>(nullable: false),
                    IdSectieDeVotare = table.Column<int>(nullable: false),
                    DataUltimeiModificari = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()"),
                    EsteZonaUrbana = table.Column<bool>(nullable: true),
                    OraPlecarii = table.Column<DateTime>(type: "datetime", nullable: true),
                    OraSosirii = table.Column<DateTime>(type: "datetime", nullable: true),
                    PresedinteBESVEsteFemeie = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaspunsFormular_1", x => new { x.IdObservator, x.IdSectieDeVotare });
                    table.ForeignKey(
                        name: "FK_RaspunsFormular_Observator",
                        column: x => x.IdObservator,
                        principalTable: "Observator",
                        principalColumn: "IdObservator",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RaspunsFormular_SectieDeVotare_IdSectieDeVotare",
                        column: x => x.IdSectieDeVotare,
                        principalTable: "SectieDeVotare",
                        principalColumn: "IdSectieDeVotarre",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Nota",
                columns: table => new
                {
                    IdNota = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaleFisierAtasat = table.Column<string>(maxLength: 1000, nullable: true),
                    DataUltimeiModificari = table.Column<DateTime>(type: "datetime", nullable: false),
                    IdIntrebare = table.Column<int>(nullable: false),
                    IdObservator = table.Column<int>(nullable: false),
                    IdSectieDeVotare = table.Column<int>(nullable: false),
                    TextNota = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nota", x => x.IdNota);
                    table.ForeignKey(
                        name: "FK_Nota_Intrebare",
                        column: x => x.IdIntrebare,
                        principalTable: "Intrebare",
                        principalColumn: "IdIntrebare",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nota_Observator_IdObservator",
                        column: x => x.IdObservator,
                        principalTable: "Observator",
                        principalColumn: "IdObservator",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nota_SectieDeVotare_IdSectieDeVotare",
                        column: x => x.IdSectieDeVotare,
                        principalTable: "SectieDeVotare",
                        principalColumn: "IdSectieDeVotarre",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Raspuns",
                columns: table => new
                {
                    IdObservator = table.Column<int>(nullable: false),
                    IdSectieDeVotare = table.Column<int>(nullable: false),
                    IdIntrebare = table.Column<int>(nullable: false),
                    IdOptiune = table.Column<int>(nullable: false),
                    DataUltimeiModificari = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()"),
                    Value = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Raspuns", x => new { x.IdObservator, x.IdSectieDeVotare, x.IdIntrebare, x.IdOptiune });
                    table.ForeignKey(
                        name: "FK_Raspuns_Intrebare",
                        column: x => x.IdIntrebare,
                        principalTable: "Intrebare",
                        principalColumn: "IdIntrebare",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Raspuns_Observator",
                        column: x => x.IdObservator,
                        principalTable: "Observator",
                        principalColumn: "IdObservator",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Raspuns_Optiune",
                        column: x => x.IdOptiune,
                        principalTable: "Optiune",
                        principalColumn: "IdOptiune",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Raspuns_SectieDeVotare_IdSectieDeVotare",
                        column: x => x.IdSectieDeVotare,
                        principalTable: "SectieDeVotare",
                        principalColumn: "IdSectieDeVotarre",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RaspunsDisponibil",
                columns: table => new
                {
                    IdRaspunsDisponibil = table.Column<int>(nullable: false),
                    IdIntrebare = table.Column<int>(nullable: false),
                    IdOptiune = table.Column<int>(nullable: false),
                    RaspunsCuFlag = table.Column<bool>(nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("IX_IdOptiune_IdIntrebare", x => x.IdRaspunsDisponibil);
                    table.ForeignKey(
                        name: "FK_RaspunsDisponibil_Intrebare",
                        column: x => x.IdIntrebare,
                        principalTable: "Intrebare",
                        principalColumn: "IdIntrebare",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RaspunsDisponibil_Optiune",
                        column: x => x.IdOptiune,
                        principalTable: "Optiune",
                        principalColumn: "IdOptiune",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccesObservatoriPerDevice_IdObservator",
                table: "AccesObservatoriPerDevice",
                column: "IdObservator");

            migrationBuilder.CreateIndex(
                name: "IX_DispozitivObservator_IdObservator",
                table: "DispozitivObservator",
                column: "IdObservator");

            migrationBuilder.CreateIndex(
                name: "IX_Intrebare_IdSectiune",
                table: "Intrebare",
                column: "IdSectiune");

            migrationBuilder.CreateIndex(
                name: "IX_Nota_IdIntrebare",
                table: "Nota",
                column: "IdIntrebare");

            migrationBuilder.CreateIndex(
                name: "IX_Nota_IdObservator",
                table: "Nota",
                column: "IdObservator");

            migrationBuilder.CreateIndex(
                name: "IX_Nota_IdSectieDeVotare",
                table: "Nota",
                column: "IdSectieDeVotare");

            migrationBuilder.CreateIndex(
                name: "IX_Observator_IdOng",
                table: "Observator",
                column: "IdOng");

            migrationBuilder.CreateIndex(
                name: "IX_Raspuns_IdIntrebare",
                table: "Raspuns",
                column: "IdIntrebare");

            migrationBuilder.CreateIndex(
                name: "IX_Raspuns_IdObservator",
                table: "Raspuns",
                column: "IdObservator");

            migrationBuilder.CreateIndex(
                name: "IX_Raspuns_IdOptiune",
                table: "Raspuns",
                column: "IdOptiune");

            migrationBuilder.CreateIndex(
                name: "IX_Raspuns_IdSectieDeVotare",
                table: "Raspuns",
                column: "IdSectieDeVotare");

            migrationBuilder.CreateIndex(
                name: "IX_RaspunsDisponibil_IdIntrebare",
                table: "RaspunsDisponibil",
                column: "IdIntrebare");

            migrationBuilder.CreateIndex(
                name: "IX_RaspunsDisponibil_IdOptiune",
                table: "RaspunsDisponibil",
                column: "IdOptiune");

            migrationBuilder.CreateIndex(
                name: "IX_RaspunsFormular_IdObservator",
                table: "RaspunsFormular",
                column: "IdObservator");

            migrationBuilder.CreateIndex(
                name: "IX_RaspunsFormular_IdSectieDeVotare",
                table: "RaspunsFormular",
                column: "IdSectieDeVotare");

            migrationBuilder.CreateIndex(
                name: "IX_SectieDeVotare_IdJudet",
                table: "SectieDeVotare",
                column: "IdJudet");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccesObservatoriPerDevice");

            migrationBuilder.DropTable(
                name: "DispozitivObservator");

            migrationBuilder.DropTable(
                name: "Nota");

            migrationBuilder.DropTable(
                name: "Raspuns");

            migrationBuilder.DropTable(
                name: "RaspunsDisponibil");

            migrationBuilder.DropTable(
                name: "RaspunsFormular");

            migrationBuilder.DropTable(
                name: "VersiuneFormular");

            migrationBuilder.DropTable(
                name: "Intrebare");

            migrationBuilder.DropTable(
                name: "Optiune");

            migrationBuilder.DropTable(
                name: "Observator");

            migrationBuilder.DropTable(
                name: "SectieDeVotare");

            migrationBuilder.DropTable(
                name: "Sectiune");

            migrationBuilder.DropTable(
                name: "ONG");

            migrationBuilder.DropTable(
                name: "Judet");
        }
    }
}
