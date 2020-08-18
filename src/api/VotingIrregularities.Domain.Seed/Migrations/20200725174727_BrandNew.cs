﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VoteMonitor.Entities.Migrations
{
    public partial class BrandNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnswerQueryInfos",
                columns: table => new
                {
                    IdPollingStation = table.Column<int>(nullable: false),
                    IdObserver = table.Column<int>(nullable: false),
                    ObserverName = table.Column<string>(nullable: true),
                    PollingStation = table.Column<string>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerQueryInfo", x => new { x.IdObserver, x.IdPollingStation });
                });

            migrationBuilder.CreateTable(
                name: "ComposedStatistics",
                columns: table => new
                {
                    Label = table.Column<string>(nullable: false),
                    Code = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticiCompuse", x => new { x.Label, x.Code });
                });

            migrationBuilder.CreateTable(
                name: "Counties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    NumberOfPollingStations = table.Column<int>(nullable: false),
                    Diaspora = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    Order = table.Column<int>(nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_County", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExportModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ObserverPhone = table.Column<string>(nullable: true),
                    IdNgo = table.Column<int>(nullable: false),
                    FormCode = table.Column<string>(nullable: true),
                    QuestionText = table.Column<string>(nullable: true),
                    OptionText = table.Column<string>(nullable: true),
                    AnswerFreeText = table.Column<string>(nullable: true),
                    NoteText = table.Column<string>(nullable: true),
                    NoteAttachmentPath = table.Column<string>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: false),
                    CountyCode = table.Column<string>(nullable: true),
                    PollingStationNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 2, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: false),
                    CurrentVersion = table.Column<int>(nullable: false),
                    Diaspora = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    Draft = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    Order = table.Column<int>(nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormVersion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ngos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ShortName = table.Column<string>(maxLength: 10, nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Organizer = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    IsActive = table.Column<bool>(nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NGO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Channel = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    InsertedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Options",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsFreeText = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    Text = table.Column<string>(maxLength: 1000, nullable: false),
                    Hint = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Option", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OptionsStatistics",
                columns: table => new
                {
                    Label = table.Column<string>(nullable: false),
                    Value = table.Column<int>(nullable: false),
                    Code = table.Column<int>(nullable: false),
                    Flagged = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticiOptiuni", x => x.Label);
                });

            migrationBuilder.CreateTable(
                name: "SimpleStatistics",
                columns: table => new
                {
                    Label = table.Column<string>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistici", x => x.Label);
                });

            migrationBuilder.CreateTable(
                name: "PollingStations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Address = table.Column<string>(maxLength: 500, nullable: true),
                    Coordinates = table.Column<string>(type: "varchar(200)", nullable: true),
                    AdministrativeTerritoryCode = table.Column<string>(maxLength: 100, nullable: true),
                    IdCounty = table.Column<int>(nullable: false),
                    TerritoryCode = table.Column<string>(maxLength: 100, nullable: false),
                    Number = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollingStation_County",
                        column: x => x.IdCounty,
                        principalTable: "Counties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormSections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 4, nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: false),
                    IdForm = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormSections_Forms_IdForm",
                        column: x => x.IdForm,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NgoAdmin",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IdNgo = table.Column<int>(nullable: false),
                    Account = table.Column<string>(maxLength: 50, nullable: false),
                    Password = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NgoAdminId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NgoAdmin_Ngo",
                        column: x => x.IdNgo,
                        principalTable: "Ngos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Observers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    FromTeam = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    IdNgo = table.Column<int>(nullable: false),
                    Phone = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Pin = table.Column<string>(maxLength: 100, nullable: false),
                    MobileDeviceId = table.Column<string>(type: "varchar(500)", nullable: true),
                    DeviceRegisterDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Observers_Ngos_IdNgo",
                        column: x => x.IdNgo,
                        principalTable: "Ngos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    IdSection = table.Column<int>(nullable: false),
                    QuestionType = table.Column<int>(nullable: false),
                    Text = table.Column<string>(maxLength: 200, nullable: false),
                    Hint = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_FormSections_IdSection",
                        column: x => x.IdSection,
                        principalTable: "FormSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationRecipients",
                columns: table => new
                {
                    ObserverId = table.Column<int>(nullable: false),
                    NotificationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationRecipients", x => new { x.ObserverId, x.NotificationId });
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_Observers_ObserverId",
                        column: x => x.ObserverId,
                        principalTable: "Observers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationRegistrationData",
                columns: table => new
                {
                    ObserverId = table.Column<int>(nullable: false),
                    ChannelName = table.Column<string>(maxLength: 256, nullable: false),
                    Token = table.Column<string>(maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationRegistrationData", x => new { x.ObserverId, x.ChannelName });
                    table.ForeignKey(
                        name: "FK_NotificationRegistrationData_Observers_ObserverId",
                        column: x => x.ObserverId,
                        principalTable: "Observers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollingStationInfos",
                columns: table => new
                {
                    IdObserver = table.Column<int>(nullable: false),
                    IdPollingStation = table.Column<int>(nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()"),
                    UrbanArea = table.Column<bool>(nullable: true),
                    ObserverLeaveTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    ObserverArrivalTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsPollingStationPresidentFemale = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationInfo", x => new { x.IdObserver, x.IdPollingStation });
                    table.ForeignKey(
                        name: "FK_PollingStationInfo_Observer",
                        column: x => x.IdObserver,
                        principalTable: "Observers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PollingStationInfos_PollingStations_IdPollingStation",
                        column: x => x.IdPollingStation,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttachementPath = table.Column<string>(maxLength: 1000, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    IdQuestion = table.Column<int>(nullable: true),
                    IdObserver = table.Column<int>(nullable: false),
                    IdPollingStation = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Note", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_Observers_IdObserver",
                        column: x => x.IdObserver,
                        principalTable: "Observers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notes_PollingStations_IdPollingStation",
                        column: x => x.IdPollingStation,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Note_Question",
                        column: x => x.IdQuestion,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OptionsToQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdQuestion = table.Column<int>(nullable: false),
                    IdOption = table.Column<int>(nullable: false),
                    Flagged = table.Column<bool>(nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionToQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionToQuestion_Option",
                        column: x => x.IdOption,
                        principalTable: "Options",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OptionToQuestion_Question",
                        column: x => x.IdQuestion,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    IdObserver = table.Column<int>(nullable: false),
                    IdOptionToQuestion = table.Column<int>(nullable: false),
                    IdPollingStation = table.Column<int>(nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()"),
                    Value = table.Column<string>(maxLength: 1000, nullable: true),
                    CountyCode = table.Column<string>(maxLength: 2, nullable: true),
                    PollingStationNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answer", x => new { x.IdObserver, x.IdOptionToQuestion, x.IdPollingStation });
                    table.ForeignKey(
                        name: "FK_Answer_Observer",
                        column: x => x.IdObserver,
                        principalTable: "Observers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Answers_OptionsToQuestions_IdOptionToQuestion",
                        column: x => x.IdOptionToQuestion,
                        principalTable: "OptionsToQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Answers_PollingStations_IdPollingStation",
                        column: x => x.IdPollingStation,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answer_IdObserver",
                table: "Answers",
                column: "IdObserver");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_IdOptionToQuestion",
                table: "Answers",
                column: "IdOptionToQuestion");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_IdPollingStation",
                table: "Answers",
                column: "IdPollingStation");

            migrationBuilder.CreateIndex(
                name: "IX_FormSections_IdForm",
                table: "FormSections",
                column: "IdForm");

            migrationBuilder.CreateIndex(
                name: "IX_NgoAdmin_IdNgo",
                table: "NgoAdmin",
                column: "IdNgo");

            migrationBuilder.CreateIndex(
                name: "IX_Note_IdObserver",
                table: "Notes",
                column: "IdObserver");

            migrationBuilder.CreateIndex(
                name: "IX_Note_IdPollingStation",
                table: "Notes",
                column: "IdPollingStation");

            migrationBuilder.CreateIndex(
                name: "IX_Note_IdQuestion",
                table: "Notes",
                column: "IdQuestion");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_NotificationId",
                table: "NotificationRecipients",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRegistrationData_IdObserver",
                table: "NotificationRegistrationData",
                column: "ObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRegistrationData_ObserverId_ChannelName",
                table: "NotificationRegistrationData",
                columns: new[] { "ObserverId", "ChannelName" });

            migrationBuilder.CreateIndex(
                name: "IX_Observer_IdNgo",
                table: "Observers",
                column: "IdNgo");

            migrationBuilder.CreateIndex(
                name: "IX_OptionToQuestion_Option",
                table: "OptionsToQuestions",
                column: "IdOption");

            migrationBuilder.CreateIndex(
                name: "IX_OptionToQuestion_Question",
                table: "OptionsToQuestions",
                column: "IdQuestion");

            migrationBuilder.CreateIndex(
                name: "IX_OptionToQuestion",
                table: "OptionsToQuestions",
                columns: new[] { "IdOption", "IdQuestion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInfo_IdObserver",
                table: "PollingStationInfos",
                column: "IdObserver");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInfo_IdPollingStation",
                table: "PollingStationInfos",
                column: "IdPollingStation");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStation_IdCounty",
                table: "PollingStations",
                column: "IdCounty");

            migrationBuilder.CreateIndex(
                name: "IX_Unique_IdCounty_IdPollingStation",
                table: "PollingStations",
                columns: new[] { "IdCounty", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Question_IdSection",
                table: "Questions",
                column: "IdSection");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerQueryInfos");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "ComposedStatistics");

            migrationBuilder.DropTable(
                name: "ExportModels");

            migrationBuilder.DropTable(
                name: "NgoAdmin");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "NotificationRecipients");

            migrationBuilder.DropTable(
                name: "NotificationRegistrationData");

            migrationBuilder.DropTable(
                name: "OptionsStatistics");

            migrationBuilder.DropTable(
                name: "PollingStationInfos");

            migrationBuilder.DropTable(
                name: "SimpleStatistics");

            migrationBuilder.DropTable(
                name: "OptionsToQuestions");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Observers");

            migrationBuilder.DropTable(
                name: "PollingStations");

            migrationBuilder.DropTable(
                name: "Options");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Ngos");

            migrationBuilder.DropTable(
                name: "Counties");

            migrationBuilder.DropTable(
                name: "FormSections");

            migrationBuilder.DropTable(
                name: "Forms");
        }
    }
}
