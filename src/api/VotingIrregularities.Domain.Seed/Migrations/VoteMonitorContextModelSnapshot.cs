﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VoteMonitor.Entities;

namespace VoteMonitor.Entities.Migrations
{
    [DbContext(typeof(VoteMonitorContext))]
    partial class VoteMonitorContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VoteMonitor.Entities.Answer", b =>
                {
                    b.Property<int>("IdObserver")
                        .HasColumnType("int");

                    b.Property<int>("IdOptionToQuestion")
                        .HasColumnType("int");

                    b.Property<int>("IdPollingStation")
                        .HasColumnType("int");

                    b.Property<string>("CountyCode")
                        .HasColumnType("nvarchar(2)")
                        .HasMaxLength(2);

                    b.Property<DateTime>("LastModified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    b.Property<int>("PollingStationNumber")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.HasKey("IdObserver", "IdOptionToQuestion", "IdPollingStation")
                        .HasName("PK_Answer");

                    b.HasIndex("IdObserver")
                        .HasName("IX_Answer_IdObserver");

                    b.HasIndex("IdOptionToQuestion")
                        .HasName("IX_Answer_IdOptionToQuestion");

                    b.HasIndex("IdPollingStation")
                        .HasName("IX_Answer_IdPollingStation");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("VoteMonitor.Entities.ComposedStatistics", b =>
                {
                    b.Property<string>("Label")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Code")
                        .HasColumnType("int");

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("Label", "Code")
                        .HasName("PK_StatisticiCompuse");

                    b.ToTable("ComposedStatistics");
                });

            modelBuilder.Entity("VoteMonitor.Entities.County", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.Property<bool>("Diaspora")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("0");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<int>("NumberOfPollingStations")
                        .HasColumnType("int");

                    b.Property<int>("Order")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValueSql("0");

                    b.HasKey("Id")
                        .HasName("PK_County");

                    b.ToTable("Counties");
                });

            modelBuilder.Entity("VoteMonitor.Entities.ExportModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AnswerFreeText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CountyCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FormCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdNgo")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("NoteAttachmentPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NoteText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ObserverPhone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OptionText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PollingStationNumber")
                        .HasColumnType("int");

                    b.Property<string>("QuestionText")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ExportModels");
                });

            modelBuilder.Entity("VoteMonitor.Entities.Form", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasMaxLength(2)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CurrentVersion")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Diaspora")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("0");

                    b.Property<bool>("Draft")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("0");

                    b.Property<int>("Order")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValueSql("0");

                    b.HasKey("Id")
                        .HasName("PK_FormVersion");

                    b.ToTable("Forms");
                });

            modelBuilder.Entity("VoteMonitor.Entities.FormSection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(4)")
                        .HasMaxLength(4);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<int>("IdForm")
                        .HasColumnType("int");

                    b.Property<int>("OrderNumber")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK_FormSection");

                    b.HasIndex("IdForm");

                    b.ToTable("FormSections");
                });

            modelBuilder.Entity("VoteMonitor.Entities.Ngo", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("0");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<bool>("Organizer")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("0");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)")
                        .HasMaxLength(10);

                    b.HasKey("Id")
                        .HasName("PK_NGO");

                    b.ToTable("Ngos");
                });

            modelBuilder.Entity("VoteMonitor.Entities.NgoAdmin", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Account")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<int>("IdNgo")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.HasKey("Id")
                        .HasName("PK_NgoAdminId");

                    b.HasIndex("IdNgo");

                    b.ToTable("NgoAdmin");
                });

            modelBuilder.Entity("VoteMonitor.Entities.Note", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AttachementPath")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<int>("IdObserver")
                        .HasColumnType("int");

                    b.Property<int>("IdPollingStation")
                        .HasColumnType("int");

                    b.Property<int?>("IdQuestion")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("datetime");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id")
                        .HasName("PK_Note");

                    b.HasIndex("IdObserver")
                        .HasName("IX_Note_IdObserver");

                    b.HasIndex("IdPollingStation")
                        .HasName("IX_Note_IdPollingStation");

                    b.HasIndex("IdQuestion")
                        .HasName("IX_Note_IdQuestion");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("VoteMonitor.Entities.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Body")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Channel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("InsertedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("SenderAdminId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SenderAdminId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("VoteMonitor.Entities.NotificationRecipient", b =>
                {
                    b.Property<int>("ObserverId")
                        .HasColumnType("int");

                    b.Property<int>("NotificationId")
                        .HasColumnType("int");

                    b.HasKey("ObserverId", "NotificationId");

                    b.HasIndex("NotificationId");

                    b.ToTable("NotificationRecipients");
                });

            modelBuilder.Entity("VoteMonitor.Entities.NotificationRegistrationData", b =>
                {
                    b.Property<int>("ObserverId")
                        .HasColumnType("int");

                    b.Property<string>("ChannelName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(512)")
                        .HasMaxLength(512);

                    b.HasKey("ObserverId", "ChannelName")
                        .HasName("PK_NotificationRegistrationData");

                    b.HasIndex("ObserverId")
                        .HasName("IX_NotificationRegistrationData_IdObserver");

                    b.HasIndex("ObserverId", "ChannelName")
                        .HasName("IX_NotificationRegistrationData_ObserverId_ChannelName");

                    b.ToTable("NotificationRegistrationData");
                });

            modelBuilder.Entity("VoteMonitor.Entities.Observer", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeviceRegisterDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("FromTeam")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("0");

                    b.Property<int>("IdNgo")
                        .HasColumnType("int");

                    b.Property<bool>("IsTestObserver")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("0");

                    b.Property<string>("MobileDeviceId")
                        .HasColumnType("varchar(500)");

                    b.Property<int>("MobileDeviceIdType")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.Property<string>("Pin")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.HasKey("Id")
                        .HasName("PK_Observer");

                    b.HasIndex("IdNgo")
                        .HasName("IX_Observer_IdNgo");

                    b.HasIndex("MobileDeviceId");

                    b.HasIndex("MobileDeviceIdType");

                    b.ToTable("Observers");
                });

            modelBuilder.Entity("VoteMonitor.Entities.Option", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Hint")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsFreeText")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("0");

                    b.Property<int>("OrderNumber")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.HasKey("Id")
                        .HasName("PK_Option");

                    b.ToTable("Options");
                });

            modelBuilder.Entity("VoteMonitor.Entities.OptionToQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Flagged")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("0");

                    b.Property<int>("IdOption")
                        .HasColumnType("int");

                    b.Property<int>("IdQuestion")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK_OptionToQuestion");

                    b.HasIndex("IdOption")
                        .HasName("IX_OptionToQuestion_Option");

                    b.HasIndex("IdQuestion")
                        .HasName("IX_OptionToQuestion_Question");

                    b.HasIndex("IdOption", "IdQuestion")
                        .IsUnique()
                        .HasName("IX_OptionToQuestion");

                    b.ToTable("OptionsToQuestions");
                });

            modelBuilder.Entity("VoteMonitor.Entities.OptionsStatistics", b =>
                {
                    b.Property<string>("Label")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Code")
                        .HasColumnType("int");

                    b.Property<bool>("Flagged")
                        .HasColumnType("bit");

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("Label")
                        .HasName("PK_StatisticiOptiuni");

                    b.ToTable("OptionsStatistics");
                });

            modelBuilder.Entity("VoteMonitor.Entities.PollingStation", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(500)")
                        .HasMaxLength(500);

                    b.Property<string>("AdministrativeTerritoryCode")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Coordinates")
                        .HasColumnType("varchar(200)");

                    b.Property<int>("IdCounty")
                        .HasColumnType("int");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.Property<string>("TerritoryCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.HasKey("Id")
                        .HasName("PK_PollingStation");

                    b.HasIndex("IdCounty")
                        .HasName("IX_PollingStation_IdCounty");

                    b.HasIndex("IdCounty", "Id")
                        .IsUnique()
                        .HasName("IX_Unique_IdCounty_IdPollingStation");

                    b.ToTable("PollingStations");
                });

            modelBuilder.Entity("VoteMonitor.Entities.PollingStationInfo", b =>
                {
                    b.Property<int>("IdObserver")
                        .HasColumnType("int");

                    b.Property<int>("IdPollingStation")
                        .HasColumnType("int");

                    b.Property<bool?>("IsPollingStationPresidentFemale")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastModified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    b.Property<DateTime?>("ObserverArrivalTime")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ObserverLeaveTime")
                        .HasColumnType("datetime");

                    b.Property<bool?>("UrbanArea")
                        .HasColumnType("bit");

                    b.HasKey("IdObserver", "IdPollingStation")
                        .HasName("PK_PollingStationInfo");

                    b.HasIndex("IdObserver")
                        .HasName("IX_PollingStationInfo_IdObserver");

                    b.HasIndex("IdPollingStation")
                        .HasName("IX_PollingStationInfo_IdPollingStation");

                    b.ToTable("PollingStationInfos");
                });

            modelBuilder.Entity("VoteMonitor.Entities.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hint")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdSection")
                        .HasColumnType("int");

                    b.Property<int>("OrderNumber")
                        .HasColumnType("int");

                    b.Property<int>("QuestionType")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.HasKey("Id")
                        .HasName("PK_Question");

                    b.HasIndex("IdSection")
                        .HasName("IX_Question_IdSection");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("VoteMonitor.Entities.SimpleStatistics", b =>
                {
                    b.Property<string>("Label")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("Label")
                        .HasName("PK_Statistici");

                    b.ToTable("SimpleStatistics");
                });

            modelBuilder.Entity("VoteMonitor.Entities.VoteMonitorContext+AnswerQueryInfo", b =>
                {
                    b.Property<int>("IdObserver")
                        .HasColumnType("int");

                    b.Property<int>("IdPollingStation")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("ObserverName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PollingStation")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdObserver", "IdPollingStation")
                        .HasName("PK_AnswerQueryInfo");

                    b.ToTable("AnswerQueryInfos");
                });

            modelBuilder.Entity("VoteMonitor.Entities.Answer", b =>
                {
                    b.HasOne("VoteMonitor.Entities.Observer", "Observer")
                        .WithMany("Answers")
                        .HasForeignKey("IdObserver")
                        .HasConstraintName("FK_Answer_Observer")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VoteMonitor.Entities.OptionToQuestion", "OptionAnswered")
                        .WithMany("Answers")
                        .HasForeignKey("IdOptionToQuestion")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VoteMonitor.Entities.PollingStation", "PollingStation")
                        .WithMany("Answers")
                        .HasForeignKey("IdPollingStation")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("VoteMonitor.Entities.FormSection", b =>
                {
                    b.HasOne("VoteMonitor.Entities.Form", "Form")
                        .WithMany("FormSections")
                        .HasForeignKey("IdForm")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VoteMonitor.Entities.NgoAdmin", b =>
                {
                    b.HasOne("VoteMonitor.Entities.Ngo", "Ngo")
                        .WithMany("NgoAdmins")
                        .HasForeignKey("IdNgo")
                        .HasConstraintName("FK_NgoAdmin_Ngo")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("VoteMonitor.Entities.Note", b =>
                {
                    b.HasOne("VoteMonitor.Entities.Observer", "Observer")
                        .WithMany("Notes")
                        .HasForeignKey("IdObserver")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VoteMonitor.Entities.PollingStation", "PollingStation")
                        .WithMany("Notes")
                        .HasForeignKey("IdPollingStation")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VoteMonitor.Entities.Question", "Question")
                        .WithMany("Notes")
                        .HasForeignKey("IdQuestion")
                        .HasConstraintName("FK_Note_Question");
                });

            modelBuilder.Entity("VoteMonitor.Entities.Notification", b =>
                {
                    b.HasOne("VoteMonitor.Entities.NgoAdmin", "SenderAdmin")
                        .WithMany("NotificationsSent")
                        .HasForeignKey("SenderAdminId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("VoteMonitor.Entities.NotificationRecipient", b =>
                {
                    b.HasOne("VoteMonitor.Entities.Notification", "Notification")
                        .WithMany("NotificationRecipients")
                        .HasForeignKey("NotificationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VoteMonitor.Entities.Observer", "Observer")
                        .WithMany("Notifications")
                        .HasForeignKey("ObserverId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("VoteMonitor.Entities.NotificationRegistrationData", b =>
                {
                    b.HasOne("VoteMonitor.Entities.Observer", "Observer")
                        .WithMany()
                        .HasForeignKey("ObserverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VoteMonitor.Entities.Observer", b =>
                {
                    b.HasOne("VoteMonitor.Entities.Ngo", "Ngo")
                        .WithMany("Observers")
                        .HasForeignKey("IdNgo")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("VoteMonitor.Entities.OptionToQuestion", b =>
                {
                    b.HasOne("VoteMonitor.Entities.Option", "Option")
                        .WithMany("OptionsToQuestions")
                        .HasForeignKey("IdOption")
                        .HasConstraintName("FK_OptionToQuestion_Option")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VoteMonitor.Entities.Question", "Question")
                        .WithMany("OptionsToQuestions")
                        .HasForeignKey("IdQuestion")
                        .HasConstraintName("FK_OptionToQuestion_Question")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("VoteMonitor.Entities.PollingStation", b =>
                {
                    b.HasOne("VoteMonitor.Entities.County", "County")
                        .WithMany("PollingStations")
                        .HasForeignKey("IdCounty")
                        .HasConstraintName("FK_PollingStation_County")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("VoteMonitor.Entities.PollingStationInfo", b =>
                {
                    b.HasOne("VoteMonitor.Entities.Observer", "Observer")
                        .WithMany("PollingStationInfos")
                        .HasForeignKey("IdObserver")
                        .HasConstraintName("FK_PollingStationInfo_Observer")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VoteMonitor.Entities.PollingStation", "PollingStation")
                        .WithMany("PollingStationInfos")
                        .HasForeignKey("IdPollingStation")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("VoteMonitor.Entities.Question", b =>
                {
                    b.HasOne("VoteMonitor.Entities.FormSection", "FormSection")
                        .WithMany("Questions")
                        .HasForeignKey("IdSection")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
