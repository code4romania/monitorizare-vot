using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Domain.Migrations
{
    [DbContext(typeof(VotingContext))]
    [Migration("20160930153542_FirstUp")]
    partial class FirstUp
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VotingIrregularities.Domain.Models.AccesObservatoriPerDevice", b =>
                {
                    b.Property<int>("IdObservator");

                    b.Property<string>("IdDispozitivMobil")
                        .HasColumnType("varchar(500)");

                    b.Property<DateTime>("DataInregistrariiDispozitivului")
                        .HasColumnType("datetime");

                    b.HasKey("IdObservator", "IdDispozitivMobil")
                        .HasName("PK_AccesObservatoriPerDevice");

                    b.HasIndex("IdObservator")
                        .HasName("IX_AccesObservatoriPerDevice_IdObservator");

                    b.ToTable("AccesObservatoriPerDevice");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.DispozitivObservator", b =>
                {
                    b.Property<int>("IdDispozitivObservator")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("IdObservator");

                    b.Property<string>("IdentificatorUnic")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("IdDispozitivObservator")
                        .HasName("PK_DispozitivObservator");

                    b.HasIndex("IdObservator")
                        .HasName("IX_DispozitivObservator_IdObservator");

                    b.ToTable("DispozitivObservator");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.Intrebare", b =>
                {
                    b.Property<int>("IdIntrebare");

                    b.Property<string>("CodFormular")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 2);

                    b.Property<int>("IdSectiune");

                    b.Property<int>("IdTipIntrebare");

                    b.Property<string>("TextIntrebare")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("IdIntrebare")
                        .HasName("PK_Intrebare");

                    b.HasIndex("IdSectiune")
                        .HasName("IX_Intrebare_IdSectiune");

                    b.ToTable("Intrebare");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.Judet", b =>
                {
                    b.Property<int>("IdJudet");

                    b.Property<string>("CodJudet")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 4);

                    b.Property<string>("Nume")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 100);

                    b.HasKey("IdJudet")
                        .HasName("PK_Judet");

                    b.ToTable("Judet");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.Nota", b =>
                {
                    b.Property<int>("IdNota")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CaleFisierAtasat")
                        .HasAnnotation("MaxLength", 1000);

                    b.Property<DateTime>("DataUltimeiModificari")
                        .HasColumnType("datetime");

                    b.Property<int>("IdIntrebare");

                    b.Property<int>("IdObservator");

                    b.Property<int>("IdSectieDeVotare");

                    b.Property<string>("TextNota");

                    b.HasKey("IdNota")
                        .HasName("PK_Nota");

                    b.HasIndex("IdIntrebare")
                        .HasName("IX_Nota_IdIntrebare");

                    b.HasIndex("IdObservator")
                        .HasName("IX_Nota_IdObservator");

                    b.HasIndex("IdSectieDeVotare")
                        .HasName("IX_Nota_IdSectieDeVotare");

                    b.ToTable("Nota");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.Observator", b =>
                {
                    b.Property<int>("IdObservator")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("EsteDinEchipa")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("0");

                    b.Property<int>("IdOng");

                    b.Property<string>("NumarTelefon")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 20);

                    b.Property<string>("NumeIntreg")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("IdObservator")
                        .HasName("PK_Observator");

                    b.HasIndex("IdOng")
                        .HasName("IX_Observator_IdOng");

                    b.ToTable("Observator");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.Ong", b =>
                {
                    b.Property<int>("IdOng");

                    b.Property<string>("AbreviereNumeOng")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 10);

                    b.Property<string>("NumeOng")
                        .IsRequired()
                        .HasColumnName("NumeONG")
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("IdOng")
                        .HasName("PK_ONG");

                    b.ToTable("ONG");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.Optiune", b =>
                {
                    b.Property<int>("IdOptiune");

                    b.Property<bool>("SeIntroduceText")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("0");

                    b.Property<string>("TextOptiune")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 100);

                    b.HasKey("IdOptiune")
                        .HasName("PK_Optiune");

                    b.ToTable("Optiune");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.Raspuns", b =>
                {
                    b.Property<int>("IdObservator");

                    b.Property<int>("IdSectieDeVotare");

                    b.Property<int>("IdIntrebare");

                    b.Property<int>("IdOptiune");

                    b.Property<DateTime>("DataUltimeiModificari")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Value")
                        .HasAnnotation("MaxLength", 1000);

                    b.HasKey("IdObservator", "IdSectieDeVotare", "IdIntrebare", "IdOptiune")
                        .HasName("PK_Raspuns");

                    b.HasIndex("IdIntrebare")
                        .HasName("IX_Raspuns_IdIntrebare");

                    b.HasIndex("IdObservator")
                        .HasName("IX_Raspuns_IdObservator");

                    b.HasIndex("IdOptiune")
                        .HasName("IX_Raspuns_IdOptiune");

                    b.HasIndex("IdSectieDeVotare")
                        .HasName("IX_Raspuns_IdSectieDeVotare");

                    b.ToTable("Raspuns");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.RaspunsDisponibil", b =>
                {
                    b.Property<int>("IdRaspunsDisponibil");

                    b.Property<int>("IdIntrebare");

                    b.Property<int>("IdOptiune");

                    b.Property<bool>("RaspunsCuFlag")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("0");

                    b.HasKey("IdRaspunsDisponibil")
                        .HasName("IX_IdOptiune_IdIntrebare");

                    b.HasIndex("IdIntrebare")
                        .HasName("IX_RaspunsDisponibil_IdIntrebare");

                    b.HasIndex("IdOptiune")
                        .HasName("IX_RaspunsDisponibil_IdOptiune");

                    b.ToTable("RaspunsDisponibil");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.RaspunsFormular", b =>
                {
                    b.Property<int>("IdObservator");

                    b.Property<int>("IdSectieDeVotare");

                    b.Property<DateTime>("DataUltimeiModificari")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    b.Property<bool?>("EsteZonaUrbana");

                    b.Property<DateTime?>("OraPlecarii")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("OraSosirii")
                        .HasColumnType("datetime");

                    b.Property<bool?>("PresedinteBesvesteFemeie")
                        .HasColumnName("PresedinteBESVEsteFemeie");

                    b.HasKey("IdObservator", "IdSectieDeVotare")
                        .HasName("PK_RaspunsFormular_1");

                    b.HasIndex("IdObservator")
                        .HasName("IX_RaspunsFormular_IdObservator");

                    b.HasIndex("IdSectieDeVotare")
                        .HasName("IX_RaspunsFormular_IdSectieDeVotare");

                    b.ToTable("RaspunsFormular");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.SectieDeVotare", b =>
                {
                    b.Property<int>("IdSectieDeVotarre")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdresaSectie")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("Coordonate")
                        .HasColumnType("varchar(200)");

                    b.Property<string>("DenumireUat")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<int>("IdJudet");

                    b.Property<string>("LocalitateComponenta")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 100);

                    b.Property<int>("NumarSectie");

                    b.HasKey("IdSectieDeVotarre")
                        .HasName("PK_SectieDeVotare");

                    b.HasIndex("IdJudet")
                        .HasName("IX_SectieDeVotare_IdJudet");

                    b.ToTable("SectieDeVotare");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.Sectiune", b =>
                {
                    b.Property<int>("IdSectiune");

                    b.Property<string>("CodSectiune")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 4);

                    b.Property<string>("Descriere")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("IdSectiune")
                        .HasName("PK_Sectiune");

                    b.ToTable("Sectiune");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.VersiuneFormular", b =>
                {
                    b.Property<string>("CodFormular")
                        .HasAnnotation("MaxLength", 2);

                    b.Property<int>("VersiuneaCurenta");

                    b.HasKey("CodFormular")
                        .HasName("PK_VersiuneFormular");

                    b.ToTable("VersiuneFormular");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.AccesObservatoriPerDevice", b =>
                {
                    b.HasOne("VotingIrregularities.Domain.Models.Observator", "IdObservatorNavigation")
                        .WithMany("AccesObservatoriPerDevice")
                        .HasForeignKey("IdObservator");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.DispozitivObservator", b =>
                {
                    b.HasOne("VotingIrregularities.Domain.Models.Observator", "IdObservatorNavigation")
                        .WithMany("DispozitivObservator")
                        .HasForeignKey("IdObservator");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.Intrebare", b =>
                {
                    b.HasOne("VotingIrregularities.Domain.Models.Sectiune", "IdSectiuneNavigation")
                        .WithMany("Intrebare")
                        .HasForeignKey("IdSectiune");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.Nota", b =>
                {
                    b.HasOne("VotingIrregularities.Domain.Models.Intrebare", "IdIntrebareNavigation")
                        .WithMany("Nota")
                        .HasForeignKey("IdIntrebare")
                        .HasConstraintName("FK_Nota_Intrebare");

                    b.HasOne("VotingIrregularities.Domain.Models.Observator", "IdObservatorNavigation")
                        .WithMany("Nota")
                        .HasForeignKey("IdObservator");

                    b.HasOne("VotingIrregularities.Domain.Models.SectieDeVotare", "IdSectieDeVotareNavigation")
                        .WithMany("Nota")
                        .HasForeignKey("IdSectieDeVotare");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.Observator", b =>
                {
                    b.HasOne("VotingIrregularities.Domain.Models.Ong", "IdOngNavigation")
                        .WithMany("Observator")
                        .HasForeignKey("IdOng");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.Raspuns", b =>
                {
                    b.HasOne("VotingIrregularities.Domain.Models.Intrebare", "IdIntrebareNavigation")
                        .WithMany("Raspuns")
                        .HasForeignKey("IdIntrebare")
                        .HasConstraintName("FK_Raspuns_Intrebare");

                    b.HasOne("VotingIrregularities.Domain.Models.Observator", "IdObservatorNavigation")
                        .WithMany("Raspuns")
                        .HasForeignKey("IdObservator")
                        .HasConstraintName("FK_Raspuns_Observator");

                    b.HasOne("VotingIrregularities.Domain.Models.Optiune", "IdOptiuneNavigation")
                        .WithMany("Raspuns")
                        .HasForeignKey("IdOptiune")
                        .HasConstraintName("FK_Raspuns_Optiune");

                    b.HasOne("VotingIrregularities.Domain.Models.SectieDeVotare", "IdSectieDeVotareNavigation")
                        .WithMany("Raspuns")
                        .HasForeignKey("IdSectieDeVotare");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.RaspunsDisponibil", b =>
                {
                    b.HasOne("VotingIrregularities.Domain.Models.Intrebare", "IdIntrebareNavigation")
                        .WithMany("RaspunsDisponibil")
                        .HasForeignKey("IdIntrebare")
                        .HasConstraintName("FK_RaspunsDisponibil_Intrebare");

                    b.HasOne("VotingIrregularities.Domain.Models.Optiune", "IdOptiuneNavigation")
                        .WithMany("RaspunsDisponibil")
                        .HasForeignKey("IdOptiune")
                        .HasConstraintName("FK_RaspunsDisponibil_Optiune");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.RaspunsFormular", b =>
                {
                    b.HasOne("VotingIrregularities.Domain.Models.Observator", "IdObservatorNavigation")
                        .WithMany("RaspunsFormular")
                        .HasForeignKey("IdObservator")
                        .HasConstraintName("FK_RaspunsFormular_Observator");

                    b.HasOne("VotingIrregularities.Domain.Models.SectieDeVotare", "IdSectieDeVotareNavigation")
                        .WithMany("RaspunsFormular")
                        .HasForeignKey("IdSectieDeVotare");
                });

            modelBuilder.Entity("VotingIrregularities.Domain.Models.SectieDeVotare", b =>
                {
                    b.HasOne("VotingIrregularities.Domain.Models.Judet", "IdJudetNavigation")
                        .WithMany("SectieDeVotare")
                        .HasForeignKey("IdJudet")
                        .HasConstraintName("FK_SectieDeVotare_Judet");
                });
        }
    }
}
