using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VoteMonitor.Entities
{
	public partial class InitialMigrationScript : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
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
				name: "Forms",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
					.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					Code = table.Column<string>(maxLength: 2, nullable: false),
					CurrentVersion = table.Column<int>(nullable: false),
					Description = table.Column<string>(nullable: true),
					Diaspora = table.Column<bool>(nullable: false, defaultValueSql: "0")
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Forms", x => x.Id);
				});

			migrationBuilder.CreateTable(
			   name: "FormSections",
			   columns: table => new
			   {
				   Id = table.Column<int>(nullable: false)
				   .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
				   Code = table.Column<string>(maxLength: 4, nullable: false),
				   Description = table.Column<string>(maxLength: 200, nullable: false),
				   IdForm = table.Column<int>()
			   },
			   constraints: table =>
			   {
				   table.PrimaryKey("PK_FormSection", x => x.Id);
				   table.ForeignKey(
					   name: "FK_Forms_FormeSections",
					   column: x => x.IdForm,
					   principalTable: "Forms",
					   principalColumn: "Id",
					   onDelete: ReferentialAction.Restrict);
			   });

			migrationBuilder.CreateTable(
				name: "Questions",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
					.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
				name: "Options",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
					.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					IsFreeText = table.Column<bool>(nullable: false, defaultValueSql: "0"),
					Text = table.Column<string>(maxLength: 1000, nullable: false),
					Hint = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Option", x => x.Id);
				});
			migrationBuilder.CreateTable(
				name: "OptionsToQuestions",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
					.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
				name: "Notes",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
							name: "FK_NotificationRegistrationData_Observer",
							column: x => x.ObserverId,
							principalTable: "Observers",
							principalColumn: "Id",
							onDelete: ReferentialAction.Restrict);
					});

            migrationBuilder.CreateTable(
            name: "Notifications",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false).
                Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                Title = table.Column<string>(maxLength: 256, nullable: false),
                Channel = table.Column<string>(maxLength: 512, nullable: false),
                Body = table.Column<string>(maxLength: 512, nullable: false),
                InsertedAt = table.Column<DateTime>()
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Notifications", x => new { x.Id });
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
                            name: "FK_NotificationRecipients_Observer",
                            column: x => x.ObserverId,
                            principalTable: "Observers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Restrict);
                        table.ForeignKey(
                            name: "FK_NotificationRecipients_Notifications",
                            column: x => x.NotificationId,
                            principalTable: "Notifications",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Restrict);
                    });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRegistrationData_ObserverId_ChannelName",
                table: "NotificationRegistrationData",
                columns: new String[] { "ObserverId", "ChannelName" },
                unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_NotificationRegistrationData_IdObserver",
				table: "NotificationRegistrationData",
				column: "ObserverId");

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
				name: "Answers");

			migrationBuilder.DropTable(
				name: "FormVersions");

			migrationBuilder.DropTable(
				name: "NgoAdmin");

			migrationBuilder.DropTable(
				name: "Notes");

			migrationBuilder.DropTable(
				name: "PollingStationInfos");

			migrationBuilder.DropTable(
				name: "OptionsToQuestions");

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
                name: "NotificationRegistrationData");

            migrationBuilder.DropTable("NotificationRecipients");
            migrationBuilder.DropTable("Notifications");
        }
    }
}
