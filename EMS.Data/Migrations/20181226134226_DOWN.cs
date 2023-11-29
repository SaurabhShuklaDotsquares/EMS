using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EMS.Data.Migrations
{
    public partial class DOWN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbroadPM",
                columns: table => new
                {
                    AutoID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 400, nullable: false),
                    Email = table.Column<string>(maxLength: 400, nullable: false),
                    Country = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AbroadPM__6B232965597B3B93", x => x.AutoID);
                });

            migrationBuilder.CreateTable(
                name: "BloodGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloodGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BucketModel",
                columns: table => new
                {
                    BucketId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ModelName = table.Column<string>(maxLength: 200, nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true),
                    ModelCode = table.Column<string>(unicode: false, maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BucketModel", x => x.BucketId);
                });

            migrationBuilder.CreateTable(
                name: "Candidate",
                columns: table => new
                {
                    CandidateID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FullName = table.Column<string>(unicode: false, maxLength: 200, nullable: false),
                    Email = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    Password = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    Contact = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(nullable: false, defaultValueSql: "((1))"),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IP = table.Column<string>(unicode: false, maxLength: 50, nullable: false, defaultValueSql: "('127.0.0.1')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate", x => x.CandidateID);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    ClientId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Email = table.Column<string>(maxLength: 200, nullable: true),
                    MSN = table.Column<string>(maxLength: 200, nullable: true),
                    Phone = table.Column<string>(maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "ntext", nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true),
                    PMUid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "Communication",
                columns: table => new
                {
                    Cid = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "('0.0.0.0')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communication", x => x.Cid);
                });

            migrationBuilder.CreateTable(
                name: "ComponentCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrName = table.Column<string>(maxLength: 10, nullable: false),
                    CurrSign = table.Column<string>(maxLength: 3, nullable: false),
                    OrderBy = table.Column<int>(nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyThought",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Thought1 = table.Column<string>(unicode: false, maxLength: 1000, nullable: true),
                    Thought2 = table.Column<string>(unicode: false, maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyThought", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    DeptId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(nullable: true, defaultValueSql: "((1))"),
                    Deptcode = table.Column<string>(maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.DeptId);
                });

            migrationBuilder.CreateTable(
                name: "DeviceCategory",
                columns: table => new
                {
                    DeviceCategoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeviceCategoryname = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceCategory", x => x.DeviceCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "DomainType",
                columns: table => new
                {
                    DomainId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DomainName = table.Column<string>(maxLength: 100, nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IP = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DomainTy__2498D75020CCCE1C", x => x.DomainId);
                });

            migrationBuilder.CreateTable(
                name: "ElanceCredential",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ElanceClientId = table.Column<string>(maxLength: 50, nullable: true),
                    ElanceClientSecret = table.Column<string>(maxLength: 50, nullable: true),
                    ElanceCode = table.Column<string>(maxLength: 100, nullable: true),
                    Access_token = table.Column<string>(maxLength: 100, nullable: true),
                    Refresh_token = table.Column<string>(maxLength: 100, nullable: true),
                    RedirectURI = table.Column<string>(maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElanceCredential", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ElanceJobDetails",
                columns: table => new
                {
                    ElanceJobId = table.Column<decimal>(type: "numeric(18, 0)", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JobId = table.Column<decimal>(type: "numeric(18, 0)", nullable: false),
                    JobName = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    Description = table.Column<string>(type: "ntext", nullable: true),
                    Budget = table.Column<string>(maxLength: 50, nullable: true),
                    NumProposals = table.Column<int>(nullable: true),
                    PostedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ClientUserId = table.Column<decimal>(type: "numeric(18, 0)", nullable: true),
                    ClientName = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    ClientCountry = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    ClientCountryCode = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    Category = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Subcategory = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Status = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    JobURL = table.Column<string>(maxLength: 500, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    isAwarded = table.Column<bool>(nullable: true),
                    IsFirstResponse = table.Column<bool>(nullable: true),
                    IsAwardedTII = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElanceJobDetails", x => x.ElanceJobId);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeActivity",
                columns: table => new
                {
                    ActivityId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Uid = table.Column<int>(nullable: false),
                    EmployeeUid = table.Column<int>(nullable: false),
                    AppraisalId = table.Column<int>(nullable: true),
                    Type = table.Column<int>(nullable: true),
                    TypeId = table.Column<int>(nullable: true),
                    TypeAns = table.Column<int>(nullable: true),
                    Comments = table.Column<string>(type: "ntext", nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "('0.0.0.0')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeActivity", x => x.ActivityId);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeFeedbackRank",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeFeedbackRank", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeFeedbackReason",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeFeedbackReason", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Examination",
                columns: table => new
                {
                    ExamID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExamCode = table.Column<string>(maxLength: 200, nullable: false),
                    MaxTime = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValueSql: "((1))"),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IP = table.Column<string>(unicode: false, maxLength: 50, nullable: false, defaultValueSql: "('127.0.0.1')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Examinat__297521A71D66518C", x => x.ExamID);
                });

            migrationBuilder.CreateTable(
                name: "FrontMenu",
                columns: table => new
                {
                    MenuId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ParentId = table.Column<int>(nullable: true),
                    MenuName = table.Column<string>(maxLength: 200, nullable: true),
                    MenuDisplayName = table.Column<string>(maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(nullable: true, defaultValueSql: "((1))"),
                    DateAdded = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    DateModify = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IP = table.Column<string>(maxLength: 50, nullable: true),
                    PageName = table.Column<string>(maxLength: 200, nullable: true),
                    ChildPages = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.MenuId);
                });

            migrationBuilder.CreateTable(
                name: "IntwExperience",
                columns: table => new
                {
                    IntwExperienceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Experience = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntwExperience", x => x.IntwExperienceId);
                });

            migrationBuilder.CreateTable(
                name: "IntwQuestype",
                columns: table => new
                {
                    IntwQuestypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TypeName = table.Column<string>(maxLength: 50, nullable: true),
                    Marks = table.Column<decimal>(type: "decimal(12, 2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntwQuestype", x => x.IntwQuestypeId);
                });

            migrationBuilder.CreateTable(
                name: "IntwTechnology",
                columns: table => new
                {
                    IntwTechnologyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TechnologyName = table.Column<string>(maxLength: 50, nullable: true),
                    NoOfQues = table.Column<int>(nullable: true),
                    NoOfMultipleQues = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntwTechnology", x => x.IntwTechnologyId);
                });

            migrationBuilder.CreateTable(
                name: "LeadClient",
                columns: table => new
                {
                    LeadClientId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 300, nullable: false),
                    Email = table.Column<string>(maxLength: 200, nullable: false),
                    PMUid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadClient", x => x.LeadClientId);
                });

            migrationBuilder.CreateTable(
                name: "Leadership",
                columns: table => new
                {
                    Lid = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "('0.0.0.0')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leadership", x => x.Lid);
                });

            migrationBuilder.CreateTable(
                name: "LeadStatus",
                columns: table => new
                {
                    StatusId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StatusName = table.Column<string>(maxLength: 300, nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    MailContent = table.Column<string>(type: "ntext", nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IP = table.Column<string>(maxLength: 50, nullable: false, defaultValueSql: "('127.0.0.1')"),
                    FromEmail = table.Column<string>(maxLength: 100, nullable: true),
                    To = table.Column<string>(maxLength: 500, nullable: true),
                    CC = table.Column<string>(maxLength: 500, nullable: true),
                    BCC = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LeadStat__C8EE20634F67C174", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "Management",
                columns: table => new
                {
                    Mid = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "('0.0.0.0')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Management", x => x.Mid);
                });

            migrationBuilder.CreateTable(
                name: "MeetingMinutes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MeetingSubject = table.Column<string>(maxLength: 1000, nullable: true),
                    MeetingDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    PMandTL = table.Column<string>(maxLength: 700, nullable: true),
                    Discussed = table.Column<string>(type: "ntext", nullable: true),
                    ActionPoint = table.Column<string>(type: "ntext", nullable: true),
                    IsDelete = table.Column<bool>(nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingMinutes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfficialLeave",
                columns: table => new
                {
                    LeaveId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    LeaveDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CountryId = table.Column<byte>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    LeaveType = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Official__796DB9593548C815", x => x.LeaveId);
                });

            migrationBuilder.CreateTable(
                name: "PersonalDevelopment",
                columns: table => new
                {
                    PdId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "('0.0.0.0')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalDevelopment", x => x.PdId);
                });

            migrationBuilder.CreateTable(
                name: "PFReviewQuarter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    QuarterName = table.Column<string>(maxLength: 50, nullable: true),
                    StartMonth = table.Column<int>(nullable: false),
                    EndMonth = table.Column<int>(nullable: false),
                    QuarterNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PFReviewQuarter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PFReviewQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReviewCategory = table.Column<string>(maxLength: 250, nullable: true),
                    ReviewQuestion = table.Column<string>(nullable: true),
                    SkillType = table.Column<byte>(nullable: false),
                    RoleType = table.Column<byte>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PFReviewQuestion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Preferences",
                columns: table => new
                {
                    PreferenceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    pmid = table.Column<int>(nullable: true),
                    PriorLeaveDay = table.Column<int>(nullable: true),
                    ActivityRefreshTime = table.Column<int>(nullable: true),
                    EmailFrom = table.Column<string>(maxLength: 3000, nullable: true),
                    EmailPM = table.Column<string>(maxLength: 3000, nullable: true),
                    EmailHR = table.Column<string>(maxLength: 3000, nullable: true),
                    IsActive = table.Column<bool>(nullable: false, defaultValueSql: "((1))"),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    EmailDeveloper = table.Column<string>(unicode: false, maxLength: 3000, nullable: true),
                    InductionDoc = table.Column<string>(maxLength: 200, nullable: true),
                    ELActTimeLimit = table.Column<int>(nullable: true),
                    TimeSheetDay = table.Column<int>(nullable: true),
                    TimeSheetEmail = table.Column<string>(maxLength: 3000, nullable: true),
                    ProjectClosureEmail = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Preferen__E228496F0E04126B", x => x.PreferenceId);
                });

            migrationBuilder.CreateTable(
                name: "Productivity",
                columns: table => new
                {
                    Pid = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "('0.0.0.0')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productivity", x => x.Pid);
                });

            migrationBuilder.CreateTable(
                name: "ProjectClose",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClosureID = table.Column<string>(maxLength: 50, nullable: true),
                    CRMID = table.Column<string>(maxLength: 50, nullable: true),
                    ClientName = table.Column<string>(maxLength: 250, nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectClose", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Qid = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 500, nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "('0.0.0.0')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Qid);
                });

            migrationBuilder.CreateTable(
                name: "Relationship",
                columns: table => new
                {
                    Rid = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "('0.0.0.0')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relationships", x => x.Rid);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleName = table.Column<string>(maxLength: 50, nullable: true),
                    MenuAccess = table.Column<string>(maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(nullable: true, defaultValueSql: "((1))"),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "TaskStatus",
                columns: table => new
                {
                    TaskStatusID = table.Column<int>(nullable: false),
                    TaskStatus = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatus", x => x.TaskStatusID);
                });

            migrationBuilder.CreateTable(
                name: "Technology",
                columns: table => new
                {
                    TechId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 50, nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(nullable: true, defaultValueSql: "((0))"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technology", x => x.TechId);
                });

            migrationBuilder.CreateTable(
                name: "TypeMaster",
                columns: table => new
                {
                    TypeId = table.Column<int>(nullable: false),
                    TypeName = table.Column<string>(maxLength: 200, nullable: false),
                    TypeGroup = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TypeMast__516F03B52A363CC5", x => x.TypeId);
                });

            migrationBuilder.CreateTable(
                name: "VirtualDeveloper",
                columns: table => new
                {
                    VirtualDeveloper_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VirtualDeveloper_Name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Skype_id = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    isactive = table.Column<bool>(nullable: true),
                    emailid = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Ismain = table.Column<bool>(nullable: true),
                    PMUid = table.Column<int>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualDeveloper", x => x.VirtualDeveloper_ID);
                });

            migrationBuilder.CreateTable(
                name: "CompanyDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Heading = table.Column<string>(maxLength: 250, nullable: true),
                    DocumentName = table.Column<string>(maxLength: 250, nullable: true),
                    DocumentPath = table.Column<string>(maxLength: 500, nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime", nullable: false),
                    DepartmentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK__CompanyDo__Depar__530E3526",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "DeptId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CurrentOpening",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DepartmentId = table.Column<int>(nullable: true),
                    Post = table.Column<string>(maxLength: 150, nullable: true),
                    Technology = table.Column<string>(maxLength: 250, nullable: true),
                    Min_Experience = table.Column<string>(maxLength: 50, nullable: true),
                    Small_Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IP = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentOpening", x => x.Id);
                    table.ForeignKey(
                        name: "FK__CurrentOp__Depar__18AC8967",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "DeptId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CadidateExam",
                columns: table => new
                {
                    CExamID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CandidateID = table.Column<int>(nullable: false),
                    ExamID = table.Column<int>(nullable: false),
                    DateOfExam = table.Column<DateTime>(type: "date", nullable: false),
                    IsComplete = table.Column<bool>(nullable: false),
                    IP = table.Column<string>(unicode: false, maxLength: 50, nullable: false, defaultValueSql: "('127.0.0.1')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cadidate__D484C60C3449B6E4", x => x.CExamID);
                    table.ForeignKey(
                        name: "FK__CadidateE__Candi__3631FF56",
                        column: x => x.CandidateID,
                        principalTable: "Candidate",
                        principalColumn: "CandidateID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__CadidateE__ExamI__3726238F",
                        column: x => x.ExamID,
                        principalTable: "Examination",
                        principalColumn: "ExamID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IntwQues",
                columns: table => new
                {
                    IntwQuesId = table.Column<decimal>(type: "numeric(18, 0)", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IntwTechnologyId = table.Column<int>(nullable: true),
                    IntwQuestypeId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Modifydate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntwQues", x => x.IntwQuesId);
                    table.ForeignKey(
                        name: "FK_IntwQues_IntwQuestypeId",
                        column: x => x.IntwQuestypeId,
                        principalTable: "IntwQuestype",
                        principalColumn: "IntwQuestypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntwQues_IntwTechnologyId",
                        column: x => x.IntwTechnologyId,
                        principalTable: "IntwTechnology",
                        principalColumn: "IntwTechnologyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IntwUser",
                columns: table => new
                {
                    IntwUserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IntwTechnologyId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    EmailID = table.Column<string>(maxLength: 100, nullable: true),
                    Mobile = table.Column<string>(maxLength: 50, nullable: true),
                    Address = table.Column<string>(nullable: true),
                    TotalAttempt = table.Column<int>(nullable: true),
                    UserResume = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Modifydate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntwUser", x => x.IntwUserId);
                    table.ForeignKey(
                        name: "FK_IntwUser_IntwTechnologyId",
                        column: x => x.IntwTechnologyId,
                        principalTable: "IntwTechnology",
                        principalColumn: "IntwTechnologyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuAccess",
                columns: table => new
                {
                    AccessId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(nullable: false),
                    MenuId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuAccess", x => x.AccessId);
                    table.ForeignKey(
                        name: "FK_MenuAccess_FrontMenu",
                        column: x => x.MenuId,
                        principalTable: "FrontMenu",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuAccess_Role",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                columns: table => new
                {
                    Uid = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(maxLength: 200, nullable: true),
                    Password = table.Column<string>(maxLength: 200, nullable: true),
                    Title = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    JobTitle = table.Column<string>(maxLength: 200, nullable: true),
                    DeptId = table.Column<int>(nullable: true),
                    RoleId = table.Column<int>(nullable: true),
                    TLId = table.Column<int>(nullable: true),
                    DOB = table.Column<DateTime>(type: "datetime", nullable: true),
                    JoinedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(nullable: true, defaultValueSql: "((1))"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "('0.0.0.0')"),
                    EmailOffice = table.Column<string>(maxLength: 200, nullable: true),
                    EmailPersonal = table.Column<string>(maxLength: 200, nullable: true),
                    MobileNumber = table.Column<string>(maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 50, nullable: true),
                    AlternativeNumber = table.Column<string>(maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "ntext", nullable: true),
                    SkypeId = table.Column<string>(maxLength: 200, nullable: true),
                    MarraigeDate = table.Column<DateTime>(type: "date", nullable: true),
                    Gender = table.Column<string>(unicode: false, maxLength: 1, nullable: true),
                    PMUid = table.Column<int>(nullable: true),
                    IsSuperAdmin = table.Column<bool>(nullable: true),
                    CRMUserId = table.Column<int>(nullable: true),
                    ApiPassword = table.Column<string>(maxLength: 50, nullable: true),
                    PanNumber = table.Column<string>(maxLength: 20, nullable: true),
                    AadharNumber = table.Column<string>(maxLength: 20, nullable: true),
                    PassportNumber = table.Column<string>(maxLength: 20, nullable: true),
                    BloodGroupId = table.Column<int>(nullable: true),
                    HRMId = table.Column<int>(nullable: true),
                    EmpCode = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => x.Uid);
                    table.ForeignKey(
                        name: "FK__UserLogin__Blood__521A10ED",
                        column: x => x.BloodGroupId,
                        principalTable: "BloodGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLogin_Department",
                        column: x => x.DeptId,
                        principalTable: "Department",
                        principalColumn: "DeptId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLogin_Role",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamQuestion",
                columns: table => new
                {
                    QuestionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Question = table.Column<string>(maxLength: 2000, nullable: false),
                    TechnologyID = table.Column<int>(nullable: true),
                    QuestionType = table.Column<int>(nullable: false),
                    QuestionLevel = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValueSql: "((1))"),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IP = table.Column<string>(unicode: false, maxLength: 50, nullable: false, defaultValueSql: "('127.0.0.1')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ExamQues__0DC06FAC0A537D18", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK__ExamQuest__Quest__0E240DFC",
                        column: x => x.QuestionLevel,
                        principalTable: "TypeMaster",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ExamQuest__Quest__0D2FE9C3",
                        column: x => x.QuestionType,
                        principalTable: "TypeMaster",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ExamQuest__Techn__0C3BC58A",
                        column: x => x.TechnologyID,
                        principalTable: "Technology",
                        principalColumn: "TechId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IntwQuesExp",
                columns: table => new
                {
                    IntwQuesExpId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IntwQuesId = table.Column<decimal>(type: "numeric(18, 0)", nullable: true),
                    IntwExperienceId = table.Column<int>(nullable: true),
                    Isactive = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntwQuesExp", x => x.IntwQuesExpId);
                    table.ForeignKey(
                        name: "FK_IntwQuesExp_ExperienceID",
                        column: x => x.IntwExperienceId,
                        principalTable: "IntwExperience",
                        principalColumn: "IntwExperienceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntwQuesExp_IntwQuesId",
                        column: x => x.IntwQuesId,
                        principalTable: "IntwQues",
                        principalColumn: "IntwQuesId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IntwQusOptions",
                columns: table => new
                {
                    IntwQusOptionsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IntwQuesId = table.Column<decimal>(type: "numeric(18, 0)", nullable: true),
                    OptionTitle = table.Column<string>(maxLength: 500, nullable: true),
                    Iscorrect = table.Column<bool>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntwQusOptions", x => x.IntwQusOptionsId);
                    table.ForeignKey(
                        name: "FK_IntwQusOptions_IntwQuesId",
                        column: x => x.IntwQuesId,
                        principalTable: "IntwQues",
                        principalColumn: "IntwQuesId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlertMessage",
                columns: table => new
                {
                    AlertId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Uid = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertMessage", x => x.AlertId);
                    table.ForeignKey(
                        name: "FK_AlertMessage_UserLogin",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Appraisal",
                columns: table => new
                {
                    AppraisalId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Uid = table.Column<int>(nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "date", nullable: false),
                    IsCommit = table.Column<bool>(nullable: true, defaultValueSql: "((0))"),
                    IsCommitTL = table.Column<bool>(nullable: true, defaultValueSql: "((0))"),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "('0.0.0.0')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appraisal", x => x.AppraisalId);
                    table.ForeignKey(
                        name: "FK_Appraisal_UserLogin",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AvailUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    UID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateModify = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IP = table.Column<string>(maxLength: 50, nullable: false, defaultValueSql: "('127.0.0.1')"),
                    IsCurrent = table.Column<bool>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailUser", x => x.ID);
                    table.ForeignKey(
                        name: "FK__AvailUser__UID__4B2D1C3C",
                        column: x => x.UID,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__AvailUser__UserI__4C214075",
                        column: x => x.UserID,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Component",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 250, nullable: true),
                    Type = table.Column<byte>(nullable: false),
                    ComponentCategoryId = table.Column<int>(nullable: false),
                    Tags = table.Column<string>(maxLength: 500, nullable: true),
                    ImageName = table.Column<string>(maxLength: 250, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DataUrl = table.Column<string>(nullable: true),
                    DesignImages = table.Column<string>(maxLength: 200, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedByUid = table.Column<int>(nullable: false),
                    PsdImages = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Component", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Component__Compo__5B6E70FD",
                        column: x => x.ComponentCategoryId,
                        principalTable: "ComponentCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Component__Creat__5C629536",
                        column: x => x.CreatedByUid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceDeviceInfo",
                columns: table => new
                {
                    DeviceDeviceInfoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeviceCategoryId = table.Column<int>(nullable: true),
                    DeviceDeviceInfoName = table.Column<string>(maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(nullable: true),
                    PmId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDeviceInfo", x => x.DeviceDeviceInfoId);
                    table.ForeignKey(
                        name: "FK__DeviceDev__Devic__7FF5EA36",
                        column: x => x.DeviceCategoryId,
                        principalTable: "DeviceCategory",
                        principalColumn: "DeviceCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__DeviceDevi__PmId__097F5470",
                        column: x => x.PmId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElanceAssignedJob",
                columns: table => new
                {
                    ElanceAssignedJobId = table.Column<decimal>(type: "numeric(18, 0)", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ElanceJobId = table.Column<decimal>(type: "numeric(18, 0)", nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElanceAssignedJob", x => x.ElanceAssignedJobId);
                    table.ForeignKey(
                        name: "FK_ElanceAssignedJob_ElanceAssignedJob",
                        column: x => x.ElanceJobId,
                        principalTable: "ElanceJobDetails",
                        principalColumn: "ElanceJobId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElanceAssignedJob_UserLogin",
                        column: x => x.UserId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpLateActivity",
                columns: table => new
                {
                    ELActId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Uid = table.Column<int>(nullable: true),
                    ELActDate = table.Column<DateTime>(type: "date", nullable: false),
                    ELActTime = table.Column<string>(maxLength: 10, nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    DateModify = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IP = table.Column<string>(maxLength: 20, nullable: true, defaultValueSql: "('127.0.0.1')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EmpLateA__F7CCFFA621A0F6C4", x => x.ELActId);
                    table.ForeignKey(
                        name: "FK__EmpLateActi__Uid__23893F36",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAppraise",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    ClientComment = table.Column<string>(type: "ntext", nullable: true),
                    ClientDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    TlComment = table.Column<string>(type: "ntext", nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: true),
                    AppraiseType = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAppraise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAppraise_UserLogin2",
                        column: x => x.EmployeeId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeAppraise_UserLogin",
                        column: x => x.UserId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeComplaint",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    ClientComment = table.Column<string>(type: "ntext", nullable: true),
                    ClientDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EmpComment = table.Column<string>(type: "ntext", nullable: true),
                    EmpDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    TlComment = table.Column<string>(type: "ntext", nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: true),
                    TLDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ComplainType = table.Column<int>(nullable: true),
                    Priority = table.Column<string>(unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeComplaint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeComplaint_UserLogin1",
                        column: x => x.EmployeeId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeComplaint_UserLogin",
                        column: x => x.UserId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeFeedback",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Uid = table.Column<int>(nullable: false),
                    LeavingDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Comment = table.Column<string>(maxLength: 500, nullable: true),
                    ReviewLink = table.Column<string>(maxLength: 100, nullable: true),
                    LFProfile = table.Column<bool>(nullable: false),
                    Suggestion = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeFeedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK__EmployeeFee__Uid__64F7DB37",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeMedicalData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeCode = table.Column<string>(maxLength: 20, nullable: false),
                    Title = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Gender = table.Column<byte>(nullable: false),
                    Designation = table.Column<string>(maxLength: 200, nullable: true),
                    DOB = table.Column<DateTime>(type: "date", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    ShowRelative = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeMedicalData", x => x.Id);
                    table.ForeignKey(
                        name: "fk_Employeemedicaluser",
                        column: x => x.UserId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Forums",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    subject = table.Column<string>(maxLength: 500, nullable: true),
                    description = table.Column<string>(type: "ntext", nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    IsClosed = table.Column<bool>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Forums_UserLogin",
                        column: x => x.UserId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobReference",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentOpeningId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 150, nullable: true),
                    Email = table.Column<string>(maxLength: 150, nullable: true),
                    MobileNo = table.Column<string>(maxLength: 20, nullable: true),
                    Small_Desc = table.Column<string>(nullable: true),
                    Attacchment = table.Column<string>(maxLength: 250, nullable: true),
                    ReferBy_UserLoginId = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IP = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobReference", x => x.Id);
                    table.ForeignKey(
                        name: "FK__JobRefere__Curre__28E2F130",
                        column: x => x.CurrentOpeningId,
                        principalTable: "CurrentOpening",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__JobRefere__Refer__29D71569",
                        column: x => x.ReferBy_UserLoginId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeBase",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Instructions = table.Column<string>(type: "text", nullable: true),
                    FilePath = table.Column<string>(maxLength: 500, nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    KnowledgeType = table.Column<int>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeBase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodeLibrary_UserLogin",
                        column: x => x.UserId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LateHour",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DayOfDate = table.Column<DateTime>(type: "date", nullable: false),
                    Uid = table.Column<int>(nullable: false),
                    LateStartTimeDiff = table.Column<TimeSpan>(nullable: true),
                    EarlyLeaveTimeDiff = table.Column<TimeSpan>(nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedByUid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LateHour", x => x.Id);
                    table.ForeignKey(
                        name: "FK__LateHour__Modifi__0EEE1503",
                        column: x => x.ModifiedByUid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__LateHour__Uid__0DF9F0CA",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveAdjust",
                columns: table => new
                {
                    AdjustId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Uid = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: true),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    Reason = table.Column<string>(type: "ntext", nullable: true),
                    IsHalf = table.Column<bool>(nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateModify = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsCancel = table.Column<bool>(nullable: true, defaultValueSql: "((0))"),
                    IP = table.Column<string>(maxLength: 50, nullable: true),
                    IsCL = table.Column<bool>(nullable: true),
                    isadjust = table.Column<bool>(nullable: true),
                    CLHalfAdjustId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveAdjust", x => x.AdjustId);
                    table.ForeignKey(
                        name: "FK_LeaveAdjust_UserLogin",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PFReviewSubmitted",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReviewOn_Uid = table.Column<int>(nullable: false),
                    ReviewBy_Uid = table.Column<int>(nullable: false),
                    ReviewQuarter = table.Column<int>(nullable: false),
                    Comments = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(nullable: true, defaultValueSql: "((1))"),
                    ReviewYear = table.Column<int>(nullable: true),
                    IsSatisfied = table.Column<bool>(nullable: true),
                    score = table.Column<decimal>(type: "decimal(5, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PFReviewSubmitted", x => x.Id);
                    table.ForeignKey(
                        name: "FK__PFReviewS__Revie__114071C9",
                        column: x => x.ReviewBy_Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__PFReviewS__Revie__104C4D90",
                        column: x => x.ReviewOn_Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__PFReviewS__Revie__12349602",
                        column: x => x.ReviewQuarter,
                        principalTable: "PFReviewQuarter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Portfolio",
                columns: table => new
                {
                    PortfolioId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteName = table.Column<string>(maxLength: 200, nullable: false),
                    WebsiteUrl = table.Column<string>(maxLength: 500, nullable: false),
                    ClientName = table.Column<string>(maxLength: 100, nullable: false),
                    CRMProjectId = table.Column<int>(nullable: true),
                    IsScratch = table.Column<bool>(nullable: false),
                    IsNDA = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(unicode: false, maxLength: 1, nullable: true),
                    Notes = table.Column<string>(type: "ntext", nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 100, nullable: true),
                    Uid = table.Column<int>(nullable: true),
                    DeveloperId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolio", x => x.PortfolioId);
                    table.ForeignKey(
                        name: "FK_Portfolio_User_Project",
                        column: x => x.DeveloperId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsClosed = table.Column<bool>(nullable: false, defaultValueSql: "((1))"),
                    CRMProjectId = table.Column<int>(nullable: false),
                    Model = table.Column<int>(nullable: true),
                    EstimateTime = table.Column<int>(nullable: true),
                    BillingTeam = table.Column<string>(maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Uid = table.Column<int>(nullable: true),
                    Status = table.Column<string>(unicode: false, maxLength: 1, nullable: true),
                    Notes = table.Column<string>(type: "ntext", nullable: true),
                    ClientId = table.Column<int>(nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true),
                    ActualDevelopers = table.Column<int>(nullable: false),
                    ProjectDetailsDoc = table.Column<string>(maxLength: 100, nullable: true),
                    PMUid = table.Column<int>(nullable: true),
                    AbroadPMUid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK__Project__AbroadP__31AD415B",
                        column: x => x.AbroadPMUid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_Client",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_BucketModel",
                        column: x => x.Model,
                        principalTable: "BucketModel",
                        principalColumn: "BucketId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLead",
                columns: table => new
                {
                    LeadId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LeadClientId = table.Column<int>(nullable: true),
                    OwnerId = table.Column<int>(nullable: false),
                    CommunicatorId = table.Column<int>(nullable: false),
                    Technologies = table.Column<string>(type: "ntext", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    QuoteSubmittedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IP = table.Column<string>(maxLength: 50, nullable: false, defaultValueSql: "('127.0.0.1')"),
                    InitalRequirement = table.Column<string>(maxLength: 300, nullable: true),
                    Notes = table.Column<string>(type: "ntext", nullable: true),
                    LeadType = table.Column<int>(nullable: false),
                    Conclusion = table.Column<string>(maxLength: 1000, nullable: true),
                    Title = table.Column<string>(maxLength: 500, nullable: false),
                    TitleCheckSum = table.Column<int>(nullable: false),
                    AbroadPMID = table.Column<int>(nullable: false, defaultValueSql: "((1))"),
                    ChaseRequests = table.Column<int>(nullable: false),
                    IsNewClient = table.Column<bool>(nullable: false, defaultValueSql: "((1))"),
                    IsUnread = table.Column<bool>(nullable: false),
                    EstimateTimeinDay = table.Column<int>(nullable: true),
                    Isdelivered = table.Column<bool>(nullable: true, defaultValueSql: "((1))"),
                    StatusUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    NextChasedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Tag = table.Column<string>(maxLength: 100, nullable: true),
                    PMID = table.Column<int>(nullable: true),
                    LeadCRMId = table.Column<string>(maxLength: 20, nullable: true),
                    ProposalDocument = table.Column<string>(maxLength: 300, nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    MockupDocument = table.Column<string>(maxLength: 500, nullable: true),
                    OtherDocument = table.Column<string>(maxLength: 500, nullable: true),
                    Wireframe_MockupsDoc = table.Column<string>(maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProjectL__73EF78FA58F12BAE", x => x.LeadId);
                    table.ForeignKey(
                        name: "FK__ProjectLe__Abroa__60283922",
                        column: x => x.AbroadPMID,
                        principalTable: "AbroadPM",
                        principalColumn: "AutoID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ProjectLe__Commu__5F9E293D",
                        column: x => x.CommunicatorId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeadClientId",
                        column: x => x.LeadClientId,
                        principalTable: "LeadClient",
                        principalColumn: "LeadClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ProjectLe__LeadT__6A1BB7B0",
                        column: x => x.LeadType,
                        principalTable: "TypeMaster",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ProjectLe__Owner__5EAA0504",
                        column: x => x.OwnerId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ProjectLe__Statu__618671AF",
                        column: x => x.Status,
                        principalTable: "LeadStatus",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLeadArchive",
                columns: table => new
                {
                    LeadId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LeadClientId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 500, nullable: false),
                    TitleCheckSum = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    CommunicatorId = table.Column<int>(nullable: false),
                    Technologies = table.Column<string>(type: "ntext", nullable: false),
                    AbroadPMID = table.Column<int>(nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    QuoteSubmittedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    InitalRequirement = table.Column<string>(maxLength: 300, nullable: true),
                    Notes = table.Column<string>(type: "ntext", nullable: true),
                    LeadType = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Conclusion = table.Column<string>(maxLength: 1000, nullable: true),
                    ChaseRequests = table.Column<int>(nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IP = table.Column<string>(maxLength: 50, nullable: false, defaultValueSql: "('127.0.0.1')"),
                    IsNewClient = table.Column<bool>(nullable: false, defaultValueSql: "((1))"),
                    IsUnread = table.Column<bool>(nullable: false),
                    EstimateTimeinDay = table.Column<int>(nullable: true),
                    Isdelivered = table.Column<bool>(nullable: true),
                    StatusUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProjectL__73EF78FA3C94E422", x => x.LeadId);
                    table.ForeignKey(
                        name: "FK__ProjectLe__Abroa__4159993F",
                        column: x => x.AbroadPMID,
                        principalTable: "AbroadPM",
                        principalColumn: "AutoID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ProjectLe__Commu__40657506",
                        column: x => x.CommunicatorId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ProjectLe__LeadC__3E7D2C94",
                        column: x => x.LeadClientId,
                        principalTable: "LeadClient",
                        principalColumn: "LeadClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ProjectLe__LeadT__424DBD78",
                        column: x => x.LeadType,
                        principalTable: "TypeMaster",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ProjectLe__Owner__3F7150CD",
                        column: x => x.OwnerId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ProjectLe__Statu__4341E1B1",
                        column: x => x.Status,
                        principalTable: "LeadStatus",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportBug",
                columns: table => new
                {
                    ReportId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SectionName = table.Column<string>(maxLength: 200, nullable: false),
                    SectionDescription = table.Column<string>(type: "ntext", nullable: false),
                    ImageName = table.Column<string>(maxLength: 200, nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IP = table.Column<string>(maxLength: 50, nullable: false, defaultValueSql: "('127.0.0.1')"),
                    UserId = table.Column<int>(nullable: true),
                    IsClosed = table.Column<bool>(nullable: true, defaultValueSql: "((0))"),
                    PagePath = table.Column<string>(maxLength: 500, nullable: true),
                    Remark = table.Column<string>(type: "ntext", nullable: true),
                    IsApproved = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportBu__D5BD480506ED0088", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK__ReportBug__UserI__0BB1B5A5",
                        column: x => x.UserId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaturdayManagement",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UID = table.Column<int>(nullable: true),
                    Month = table.Column<int>(nullable: true),
                    Year = table.Column<int>(nullable: true),
                    SaturdayDt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Ispresent = table.Column<bool>(nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedDt = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastProcessDt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaturdayManagement", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SaturdayManagement_UserLogin",
                        column: x => x.UID,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    TaskID = table.Column<decimal>(type: "numeric(18, 0)", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TaskName = table.Column<string>(maxLength: 500, nullable: true),
                    Priority = table.Column<short>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    TaskStatusID = table.Column<int>(nullable: false),
                    AddedUid = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    TaskEndDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.TaskID);
                    table.ForeignKey(
                        name: "FK__Task__AddedUid__218BE82B",
                        column: x => x.AddedUid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Task__TaskStatus__2097C3F2",
                        column: x => x.TaskStatusID,
                        principalTable: "TaskStatus",
                        principalColumn: "TaskStatusID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User_Spec",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TechId = table.Column<int>(nullable: false),
                    Uid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Spec", x => x.ID);
                    table.ForeignKey(
                        name: "FK_User_Spec_Technology",
                        column: x => x.TechId,
                        principalTable: "Technology",
                        principalColumn: "TechId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_Spec_UserLogin",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User_Tech",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TechId = table.Column<int>(nullable: false),
                    Uid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Tech", x => x.ID);
                    table.ForeignKey(
                        name: "FK_User_Tech_Project",
                        column: x => x.TechId,
                        principalTable: "Technology",
                        principalColumn: "TechId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_Tech_UserLogin",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLog",
                columns: table => new
                {
                    LogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Uid = table.Column<int>(nullable: true),
                    Login = table.Column<DateTime>(type: "datetime", nullable: true),
                    Logout = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLog", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_UserLog_UserLog",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CandidateAnswer",
                columns: table => new
                {
                    CAnswerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CQuestionId = table.Column<int>(nullable: false),
                    CAnswer = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Candidat__22973C3B3BEAD8AC", x => x.CAnswerID);
                    table.ForeignKey(
                        name: "FK__Candidate__CQues__3DD3211E",
                        column: x => x.CQuestionId,
                        principalTable: "ExamQuestion",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamQuestionAnswerDetail",
                columns: table => new
                {
                    QAId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuestID = table.Column<int>(nullable: true),
                    Answer = table.Column<string>(maxLength: 2000, nullable: false),
                    IsCorrect = table.Column<bool>(nullable: true, defaultValueSql: "((0))"),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IP = table.Column<string>(unicode: false, maxLength: 50, nullable: false, defaultValueSql: "('127.0.0.1')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ExamQues__DFA593A014D10B8B", x => x.QAId);
                    table.ForeignKey(
                        name: "FK__ExamQuest__Quest__16B953FD",
                        column: x => x.QuestID,
                        principalTable: "ExamQuestion",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamQuestionDetail",
                columns: table => new
                {
                    EQID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExamID = table.Column<int>(nullable: false),
                    QuestionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ExamQues__288E09E125077354", x => x.EQID);
                    table.ForeignKey(
                        name: "FK__ExamQuest__ExamI__27E3DFFF",
                        column: x => x.ExamID,
                        principalTable: "Examination",
                        principalColumn: "ExamID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ExamQuest__Quest__28D80438",
                        column: x => x.QuestionID,
                        principalTable: "ExamQuestion",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IntwUserSession",
                columns: table => new
                {
                    IntwUserSessionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IntwUserId = table.Column<int>(nullable: true),
                    IntwQuesExpId = table.Column<int>(nullable: true),
                    TotalQues = table.Column<int>(nullable: true),
                    TotalMarks = table.Column<int>(nullable: true),
                    MarksObtained = table.Column<int>(nullable: true),
                    TotalTime = table.Column<int>(nullable: true),
                    Result = table.Column<bool>(nullable: true),
                    Status = table.Column<string>(maxLength: 50, nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntwUserSession", x => x.IntwUserSessionId);
                    table.ForeignKey(
                        name: "FK_IntwUserSession_IntwQuesExpId",
                        column: x => x.IntwQuesExpId,
                        principalTable: "IntwQuesExp",
                        principalColumn: "IntwQuesExpId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntwUserSession_IntwUserId",
                        column: x => x.IntwUserId,
                        principalTable: "IntwUser",
                        principalColumn: "IntwUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlertMessageRead",
                columns: table => new
                {
                    AlertReadId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlertId = table.Column<int>(nullable: false),
                    Uid = table.Column<int>(nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsRead = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertMessageRead", x => x.AlertReadId);
                    table.ForeignKey(
                        name: "FK_AlertMessageRead_AlertMessage",
                        column: x => x.AlertId,
                        principalTable: "AlertMessage",
                        principalColumn: "AlertId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AlertMessageRead_UserLogin",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppraisalExtras",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppraisalId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    Comments = table.Column<string>(maxLength: 1000, nullable: true),
                    TLUserId = table.Column<int>(nullable: true),
                    TLComments = table.Column<string>(maxLength: 1000, nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppraisalExtras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppraisalExtras_Appraisal",
                        column: x => x.AppraisalId,
                        principalTable: "Appraisal",
                        principalColumn: "AppraisalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppraisalId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Qid = table.Column<int>(nullable: true),
                    Comments = table.Column<string>(type: "ntext", nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "('0.0.0.0')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAnswers_Appraisal",
                        column: x => x.AppraisalId,
                        principalTable: "Appraisal",
                        principalColumn: "AppraisalId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeAnswers_Questions",
                        column: x => x.Qid,
                        principalTable: "Questions",
                        principalColumn: "Qid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FullLeave",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    AppraisalId = table.Column<int>(nullable: true),
                    TotalLeave = table.Column<int>(nullable: true),
                    Approved = table.Column<int>(nullable: true),
                    NotApproved = table.Column<int>(nullable: true),
                    Adjustment = table.Column<int>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "('0.0.0.0')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FullLeave", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FullLeave_Appraisal",
                        column: x => x.AppraisalId,
                        principalTable: "Appraisal",
                        principalColumn: "AppraisalId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FullLeave_UserLogin",
                        column: x => x.UserId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HalfLeave",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    AppraisalId = table.Column<int>(nullable: true),
                    TotalLeave = table.Column<int>(nullable: true),
                    Approved = table.Column<int>(nullable: true),
                    NotApproved = table.Column<int>(nullable: true),
                    Adjustment = table.Column<int>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "('0.0.0.0')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HalfLeave", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HalfLeave_Appraisal",
                        column: x => x.AppraisalId,
                        principalTable: "Appraisal",
                        principalColumn: "AppraisalId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HalfLeave_UserLogin",
                        column: x => x.UserId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceDeviceHistory",
                columns: table => new
                {
                    DeviceDeviceHistoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeviceDeviceInfoId = table.Column<int>(nullable: true),
                    Uid = table.Column<int>(nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    SubmitedBy = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false, defaultValueSql: "((1))"),
                    AddedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    SubmitedTo = table.Column<int>(nullable: true),
                    SubmitApproved = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDeviceHistory", x => x.DeviceDeviceHistoryId);
                    table.ForeignKey(
                        name: "FK__DeviceDev__Devic__04BA9F53",
                        column: x => x.DeviceDeviceInfoId,
                        principalTable: "DeviceDeviceInfo",
                        principalColumn: "DeviceDeviceInfoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__DeviceDevic__Uid__05AEC38C",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeFeedbackRankStatus",
                columns: table => new
                {
                    EmployeeFeedbackRankId = table.Column<int>(nullable: false),
                    EmployeeFeedbackId = table.Column<int>(nullable: false),
                    FeedBackStatus = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeFeedbackRankStatus", x => new { x.EmployeeFeedbackRankId, x.EmployeeFeedbackId, x.FeedBackStatus });
                    table.ForeignKey(
                        name: "FK__EmployeeF__Emplo__76226739",
                        column: x => x.EmployeeFeedbackId,
                        principalTable: "EmployeeFeedback",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__EmployeeF__Emplo__752E4300",
                        column: x => x.EmployeeFeedbackRankId,
                        principalTable: "EmployeeFeedbackReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeFeedbackReasonMapping",
                columns: table => new
                {
                    EmployeeFeedbackId = table.Column<int>(nullable: false),
                    EmployeeFeedbackReasonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeFeedbackReasonMapping", x => new { x.EmployeeFeedbackId, x.EmployeeFeedbackReasonId });
                    table.ForeignKey(
                        name: "FK__EmployeeF__Emplo__6F7569AA",
                        column: x => x.EmployeeFeedbackId,
                        principalTable: "EmployeeFeedback",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__EmployeeF__Emplo__70698DE3",
                        column: x => x.EmployeeFeedbackReasonId,
                        principalTable: "EmployeeFeedbackReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeRelativeMedicalData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeMedicalId = table.Column<int>(nullable: false),
                    Relation = table.Column<byte>(nullable: false),
                    Title = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Gender = table.Column<byte>(nullable: false),
                    DOB = table.Column<DateTime>(type: "date", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeRelativeMedicalData", x => x.Id);
                    table.ForeignKey(
                        name: "fk_employeereative",
                        column: x => x.EmployeeMedicalId,
                        principalTable: "EmployeeMedicalData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForumFeedback",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ForumId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    Feedback = table.Column<string>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumFeedback", x => x.id);
                    table.ForeignKey(
                        name: "FK_ForumFeedback_ForumFeedback",
                        column: x => x.ForumId,
                        principalTable: "Forums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForumFeedback_UserLogin",
                        column: x => x.UserId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReadMessage",
                columns: table => new
                {
                    MessageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ForumId = table.Column<int>(nullable: true),
                    Uid = table.Column<int>(nullable: true),
                    IsRead = table.Column<bool>(nullable: true),
                    DateRead = table.Column<DateTime>(type: "datetime", nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadMessage", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_ReadMessage_Forums",
                        column: x => x.ForumId,
                        principalTable: "Forums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReadMessage_UserLogin",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeDepartment",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeId = table.Column<int>(nullable: false),
                    DeptID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeDepartment", x => x.ID);
                    table.ForeignKey(
                        name: "FK__Knowledge__CodeI__232A17DA",
                        column: x => x.CodeId,
                        principalTable: "KnowledgeBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Knowledge__DeptI__241E3C13",
                        column: x => x.DeptID,
                        principalTable: "Department",
                        principalColumn: "DeptId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeTech",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeId = table.Column<int>(nullable: false),
                    TechId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeTech", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodeTech_CodeLibrary",
                        column: x => x.CodeId,
                        principalTable: "KnowledgeBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CodeTech_Technology",
                        column: x => x.TechId,
                        principalTable: "Technology",
                        principalColumn: "TechId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveActivity",
                columns: table => new
                {
                    LeaveId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Uid = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "ntext", nullable: true),
                    Status = table.Column<int>(nullable: true),
                    WorkAlternator = table.Column<string>(type: "ntext", nullable: true),
                    Remark = table.Column<string>(type: "ntext", nullable: true),
                    IsHalf = table.Column<bool>(nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateModify = table.Column<DateTime>(type: "datetime", nullable: true),
                    WorkAlterID = table.Column<int>(nullable: true),
                    LeaveType = table.Column<int>(nullable: true),
                    IP = table.Column<string>(maxLength: 50, nullable: true),
                    AdjustID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveStatus", x => x.LeaveId);
                    table.ForeignKey(
                        name: "FK_LeaveActivity_LeaveAdjust",
                        column: x => x.AdjustID,
                        principalTable: "LeaveAdjust",
                        principalColumn: "AdjustId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__LeaveActi__Leave__48BAC3E5",
                        column: x => x.LeaveType,
                        principalTable: "TypeMaster",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveActivity_TypeMaster",
                        column: x => x.Status,
                        principalTable: "TypeMaster",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveActivity_UserLogin",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__LeaveActi__WorkA__68687968",
                        column: x => x.WorkAlterID,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PFReviewResult",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PFReviewSubmittedId = table.Column<int>(nullable: false),
                    PFReviewQuestionId = table.Column<int>(nullable: false),
                    PFReviewAnswer = table.Column<byte>(nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PFReviewResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK__PFReviewR__PFRev__17ED6F58",
                        column: x => x.PFReviewQuestionId,
                        principalTable: "PFReviewQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__PFReviewR__PFRev__16F94B1F",
                        column: x => x.PFReviewSubmittedId,
                        principalTable: "PFReviewSubmitted",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Portfolio_Domain",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PortfolioID = table.Column<int>(nullable: false),
                    DomainId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolio_Domain", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Portfolio_Domain_Technology",
                        column: x => x.DomainId,
                        principalTable: "DomainType",
                        principalColumn: "DomainId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Portfolio_Domain_Project",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolio",
                        principalColumn: "PortfolioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Portfolio_Tech",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PortfolioID = table.Column<int>(nullable: false),
                    TechId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolio_Tech", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Portfolio_Tech_Project",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolio",
                        principalColumn: "PortfolioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Portfolio_Tech_Technology",
                        column: x => x.TechId,
                        principalTable: "Technology",
                        principalColumn: "TechId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DesignerManagement",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    DesignerDesription = table.Column<string>(unicode: false, nullable: true),
                    AssignUID = table.Column<int>(nullable: false),
                    Priority = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    TaskTime = table.Column<int>(nullable: true),
                    IsPaid = table.Column<bool>(nullable: true),
                    AddedUID = table.Column<int>(nullable: true),
                    TaskCompletedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignerManagement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DesignerManagement_UserLogin1",
                        column: x => x.AddedUID,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DesignerManagement_UserLogin",
                        column: x => x.AssignUID,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DesignerManagement_Project",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProject",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    AppraisalId = table.Column<int>(nullable: true),
                    Year = table.Column<int>(nullable: true),
                    PeriodFrom = table.Column<DateTime>(type: "date", nullable: true),
                    PeriodTo = table.Column<DateTime>(type: "date", nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    Role = table.Column<int>(nullable: true),
                    Comments = table.Column<string>(type: "ntext", nullable: true),
                    TLComments = table.Column<string>(type: "ntext", nullable: true),
                    TLStatus = table.Column<string>(maxLength: 100, nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IP = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "('0.0.0.0')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeProject_Project",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeProject_Role",
                        column: x => x.Role,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Project_Department",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectID = table.Column<int>(nullable: false),
                    DeptID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project_Department", x => x.ID);
                    table.ForeignKey(
                        name: "FK__Project_D__DeptI__1E6562BD",
                        column: x => x.DeptID,
                        principalTable: "Department",
                        principalColumn: "DeptId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Project_D__Proje__1D713E84",
                        column: x => x.ProjectID,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Project_Tech",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectID = table.Column<int>(nullable: false),
                    TechId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project_Tech", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Project_Tech_Project",
                        column: x => x.ProjectID,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_Tech_Technology",
                        column: x => x.TechId,
                        principalTable: "Technology",
                        principalColumn: "TechId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectClosure",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectID = table.Column<int>(nullable: true),
                    DateofClosing = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CRMStatus = table.Column<int>(nullable: true),
                    Uid_Dev = table.Column<int>(nullable: true),
                    OtherActualDeveloper = table.Column<string>(maxLength: 500, nullable: true),
                    Uid_BA = table.Column<int>(nullable: true),
                    Uid_TL = table.Column<int>(nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime", nullable: false),
                    NextStartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    Suggestion = table.Column<string>(nullable: true),
                    ClientQuality = table.Column<int>(nullable: true),
                    AddedBy = table.Column<int>(nullable: true),
                    Country = table.Column<string>(maxLength: 100, nullable: true),
                    PMID = table.Column<int>(nullable: true),
                    ProjectLiveUrl = table.Column<string>(maxLength: 500, nullable: true),
                    ProjectUrlAbsenseReason = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectClosure", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectClosure_UserLogin3",
                        column: x => x.AddedBy,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectClosure_Project",
                        column: x => x.ProjectID,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectClosure_UserLogin",
                        column: x => x.Uid_BA,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectClosure_UserLogin1",
                        column: x => x.Uid_Dev,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectClosure_UserLogin2",
                        column: x => x.Uid_TL,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectDeveloper",
                columns: table => new
                {
                    ProjectDeveloperID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    Uid = table.Column<int>(nullable: true),
                    TransId = table.Column<Guid>(nullable: false),
                    Remark = table.Column<string>(type: "ntext", nullable: true),
                    WorkStatus = table.Column<int>(nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IP = table.Column<string>(maxLength: 50, nullable: false),
                    VD_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectDeveloper", x => x.ProjectDeveloperID);
                    table.ForeignKey(
                        name: "FK_ProjectDeveloper_Project",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectDeveloper_UserLogin",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ProjectDe__VD_id__5CACADF9",
                        column: x => x.VD_id,
                        principalTable: "VirtualDeveloper",
                        principalColumn: "VirtualDeveloper_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectDeveloper_TypeMaster",
                        column: x => x.WorkStatus,
                        principalTable: "TypeMaster",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectDeveloperAddon",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    Uid = table.Column<int>(nullable: false),
                    TransId = table.Column<Guid>(nullable: false),
                    Remark = table.Column<string>(type: "ntext", nullable: true),
                    WorkStatus = table.Column<int>(nullable: false),
                    WorkRole = table.Column<int>(nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IP = table.Column<string>(maxLength: 50, nullable: false),
                    IsCurrent = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectDeveloperAddon", x => new { x.ProjectId, x.Uid, x.TransId });
                    table.ForeignKey(
                        name: "FK_ProjectDeveloperAddon_Project",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectDeveloperAddon_UserLogin",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectDeveloperAddon_TypeMaster1",
                        column: x => x.WorkRole,
                        principalTable: "TypeMaster",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectDeveloperAddon_TypeMaster",
                        column: x => x.WorkStatus,
                        principalTable: "TypeMaster",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectInvoice",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    InvoiceNumber = table.Column<string>(maxLength: 50, nullable: false),
                    InvoiceStartDate = table.Column<DateTime>(type: "date", nullable: false),
                    InvoiceEndDate = table.Column<DateTime>(type: "date", nullable: false),
                    InvoiceAmount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    InvoiceStatus = table.Column<int>(nullable: false),
                    Uid_BA = table.Column<int>(nullable: true),
                    Uid_TL = table.Column<int>(nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime", nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    Country = table.Column<string>(maxLength: 100, nullable: true),
                    PMID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectInvoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectInvoice_Currency",
                        column: x => x.CurrencyID,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ProjectIn__Proje__3B6BB5BF",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ProjectIn__Uid_B__3C5FD9F8",
                        column: x => x.Uid_BA,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ProjectIn__Uid_T__3D53FE31",
                        column: x => x.Uid_TL,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectPM",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProjectP__A76232342DDCB077", x => new { x.ProjectId, x.UserId });
                    table.ForeignKey(
                        name: "fk_projectpm_project",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_projectpm_user",
                        column: x => x.UserId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubProjects",
                columns: table => new
                {
                    SubProjectId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    SubProjectName = table.Column<string>(maxLength: 200, nullable: false, defaultValueSql: "('')"),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProjects", x => x.SubProjectId);
                    table.ForeignKey(
                        name: "FK_SubProjects_SubProjects",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserActivity",
                columns: table => new
                {
                    ActivityID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Uid = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(type: "date", nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    ProjectName = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<string>(maxLength: 50, nullable: true),
                    Comment = table.Column<string>(maxLength: 2000, nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateModify = table.Column<DateTime>(type: "datetime", nullable: true),
                    SubProjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivity", x => x.ActivityID);
                    table.ForeignKey(
                        name: "FK_UA_Project_ID",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserActivityCheck_UserLogin",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserActivityLog",
                columns: table => new
                {
                    ActivityLogID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Uid = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    ProjectId = table.Column<int>(nullable: true),
                    ProjectName = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<string>(maxLength: 50, nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserActi__19A9B78F41AE9EFA", x => x.ActivityLogID);
                    table.ForeignKey(
                        name: "FK_UAL_Project_ID",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserActivityLog_UserLogin",
                        column: x => x.Uid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserTimeSheet",
                columns: table => new
                {
                    UserTimeSheetID = table.Column<decimal>(type: "numeric(18, 0)", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ProjectID = table.Column<int>(nullable: false),
                    VirtualDeveloper_id = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    UID = table.Column<int>(nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    WorkHours = table.Column<TimeSpan>(nullable: false),
                    ReviewedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ReviewedByUid = table.Column<int>(nullable: true),
                    IsReviewed = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTimeSheet", x => x.UserTimeSheetID);
                    table.ForeignKey(
                        name: "FK_Project_UserTimeSheet",
                        column: x => x.ProjectID,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__UserTimeS__Revie__4B6D135E",
                        column: x => x.ReviewedByUid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLogin_UserTimeSheet",
                        column: x => x.UID,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VirtualDeveloper_UserTimeSheet",
                        column: x => x.VirtualDeveloper_id,
                        principalTable: "VirtualDeveloper",
                        principalColumn: "VirtualDeveloper_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstimateDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    Tags = table.Column<string>(maxLength: 400, nullable: true),
                    DocumentPath = table.Column<string>(maxLength: 400, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime", nullable: false),
                    Industry = table.Column<string>(maxLength: 400, nullable: true),
                    Technology = table.Column<string>(maxLength: 50, nullable: false),
                    Uid_UploadedBy = table.Column<int>(nullable: true),
                    MockupDocument = table.Column<string>(maxLength: 500, nullable: true),
                    OtherDocument = table.Column<string>(maxLength: 500, nullable: true),
                    Wireframe_MockupsDoc = table.Column<string>(maxLength: 300, nullable: true),
                    LeadId = table.Column<int>(nullable: true),
                    EstimateTimeInDays = table.Column<int>(nullable: true),
                    IsSpam = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimateDocument", x => x.Id);
                    table.ForeignKey(
                        name: "fk_lead",
                        column: x => x.LeadId,
                        principalTable: "ProjectLead",
                        principalColumn: "LeadId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UploadedBy",
                        column: x => x.Uid_UploadedBy,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectDescription = table.Column<string>(unicode: false, nullable: false),
                    LeadId = table.Column<int>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    Country = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    AddedPersonUId = table.Column<int>(nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    TentiveDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ChasingType = table.Column<int>(nullable: false),
                    Groups = table.Column<string>(maxLength: 250, nullable: true),
                    NoOfDeveloper = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: true),
                    ClientId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forecasting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Forecasting_UserLogin",
                        column: x => x.AddedPersonUId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Forecasting_Client",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Forecasting_ProjectLead",
                        column: x => x.LeadId,
                        principalTable: "ProjectLead",
                        principalColumn: "LeadId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Forecasting_Project",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeadTechnician",
                columns: table => new
                {
                    AutoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LeadId = table.Column<int>(nullable: false),
                    TechnicianId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LeadTech__6B2329057775B2CE", x => x.AutoId);
                    table.ForeignKey(
                        name: "FK__LeadTechn__LeadI__795DFB40",
                        column: x => x.LeadId,
                        principalTable: "ProjectLead",
                        principalColumn: "LeadId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__LeadTechn__Techn__7A521F79",
                        column: x => x.TechnicianId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeadTransaction",
                columns: table => new
                {
                    TransId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LeadId = table.Column<int>(nullable: false),
                    Doc = table.Column<string>(maxLength: 300, nullable: true),
                    Notes = table.Column<string>(type: "ntext", nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    AddedBy = table.Column<int>(nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LeadTran__9E5DDB3C0C70CFB4", x => x.TransId);
                    table.ForeignKey(
                        name: "FK__LeadTrans__Added__10416098",
                        column: x => x.AddedBy,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__LeadTrans__LeadI__0E591826",
                        column: x => x.LeadId,
                        principalTable: "ProjectLead",
                        principalColumn: "LeadId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__LeadTrans__Statu__0F4D3C5F",
                        column: x => x.StatusId,
                        principalTable: "LeadStatus",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLeadTech",
                columns: table => new
                {
                    ProjectLeadTechId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LeadId = table.Column<int>(nullable: false),
                    TechId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLeadTech", x => x.ProjectLeadTechId);
                    table.ForeignKey(
                        name: "FK_ProjectLeadTech_LeadId",
                        column: x => x.LeadId,
                        principalTable: "ProjectLead",
                        principalColumn: "LeadId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectLeadTech_TechId",
                        column: x => x.TechId,
                        principalTable: "Technology",
                        principalColumn: "TechId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeadTechnicianArchive",
                columns: table => new
                {
                    AutoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LeadId = table.Column<int>(nullable: false),
                    TechnicianId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LeadTech__6B2329055EE9FC26", x => x.AutoId);
                    table.ForeignKey(
                        name: "FK__LeadTechn__LeadI__60D24498",
                        column: x => x.LeadId,
                        principalTable: "ProjectLeadArchive",
                        principalColumn: "LeadId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__LeadTechn__Techn__61C668D1",
                        column: x => x.TechnicianId,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeadTransactionArchive",
                columns: table => new
                {
                    TransId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LeadId = table.Column<int>(nullable: false),
                    Doc = table.Column<string>(maxLength: 300, nullable: true),
                    Notes = table.Column<string>(type: "ntext", nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    AddedBy = table.Column<int>(nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LeadTran__9E5DDB3C5748DA5E", x => x.TransId);
                    table.ForeignKey(
                        name: "FK__LeadTrans__Added__5B196B42",
                        column: x => x.AddedBy,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__LeadTrans__LeadI__593122D0",
                        column: x => x.LeadId,
                        principalTable: "ProjectLeadArchive",
                        principalColumn: "LeadId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__LeadTrans__Statu__5A254709",
                        column: x => x.StatusId,
                        principalTable: "LeadStatus",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLeadTechArchive",
                columns: table => new
                {
                    ProjectLeadTechId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LeadId = table.Column<int>(nullable: false),
                    TechId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProjectL__B97F51824BD727B2", x => x.ProjectLeadTechId);
                    table.ForeignKey(
                        name: "FK__ProjectLe__LeadI__4DBF7024",
                        column: x => x.LeadId,
                        principalTable: "ProjectLeadArchive",
                        principalColumn: "LeadId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ProjectLe__TechI__4EB3945D",
                        column: x => x.TechId,
                        principalTable: "Technology",
                        principalColumn: "TechId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskAssignedTo",
                columns: table => new
                {
                    AssignedToTaskID = table.Column<decimal>(type: "numeric(18, 0)", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TaskID = table.Column<decimal>(type: "numeric(18, 0)", nullable: false),
                    AssignUid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TaskAssi__8B514B062B155265", x => x.AssignedToTaskID);
                    table.ForeignKey(
                        name: "FK__TaskAssig__Assig__2DF1BF10",
                        column: x => x.AssignUid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__TaskAssig__TaskI__2CFD9AD7",
                        column: x => x.TaskID,
                        principalTable: "Task",
                        principalColumn: "TaskID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskComment",
                columns: table => new
                {
                    TaskCommentID = table.Column<decimal>(type: "numeric(18, 0)", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TaskID = table.Column<decimal>(type: "numeric(18, 0)", nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    TaskStatusID = table.Column<int>(nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    AddedUid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskComment", x => x.TaskCommentID);
                    table.ForeignKey(
                        name: "FK__TaskComme__Added__3D3402A0",
                        column: x => x.AddedUid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__TaskComme__TaskI__32B6742D",
                        column: x => x.TaskID,
                        principalTable: "Task",
                        principalColumn: "TaskID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__TaskComme__TaskS__33AA9866",
                        column: x => x.TaskStatusID,
                        principalTable: "TaskStatus",
                        principalColumn: "TaskStatusID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IntwUserQues",
                columns: table => new
                {
                    IntwUserQuesId = table.Column<decimal>(type: "numeric(18, 0)", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IntwQuesid = table.Column<decimal>(type: "numeric(18, 0)", nullable: true),
                    IntwUserid = table.Column<int>(nullable: true),
                    IntwUserSessionid = table.Column<int>(nullable: true),
                    Adddate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntwUserQues", x => x.IntwUserQuesId);
                    table.ForeignKey(
                        name: "FK_IntwUserQues_IntwQues",
                        column: x => x.IntwQuesid,
                        principalTable: "IntwQues",
                        principalColumn: "IntwQuesId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntwUserQues_IntwUserSession",
                        column: x => x.IntwUserSessionid,
                        principalTable: "IntwUserSession",
                        principalColumn: "IntwUserSessionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntwUserQues_IntwUser",
                        column: x => x.IntwUserid,
                        principalTable: "IntwUser",
                        principalColumn: "IntwUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveActivityAdjust",
                columns: table => new
                {
                    LeaveActivityAdjustId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LeaveId = table.Column<int>(nullable: true),
                    Adjustid = table.Column<int>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveActivityAdjust", x => x.LeaveActivityAdjustId);
                    table.ForeignKey(
                        name: "FK_LeaveActivityAdjust_LeaveAdjust",
                        column: x => x.Adjustid,
                        principalTable: "LeaveAdjust",
                        principalColumn: "AdjustId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveActivityAdjust_LeaveActivity",
                        column: x => x.LeaveId,
                        principalTable: "LeaveActivity",
                        principalColumn: "LeaveId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectClosureDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectClosureId = table.Column<int>(nullable: true),
                    NextStartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    AddedByUid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectClosureDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK__ProjectCl__Added__382534C0",
                        column: x => x.AddedByUid,
                        principalTable: "UserLogin",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__ProjectCl__Proje__15460CD7",
                        column: x => x.ProjectClosureId,
                        principalTable: "ProjectClosure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectInvoiceComment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectInvoiceId = table.Column<int>(nullable: false),
                    InvoiceComments = table.Column<string>(nullable: true),
                    ChaseDate = table.Column<DateTime>(type: "date", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectInvoiceComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK__ProjectIn__Proje__4218B34E",
                        column: x => x.ProjectInvoiceId,
                        principalTable: "ProjectInvoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IntwUserAnswer",
                columns: table => new
                {
                    IntwUserAnswerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IntwUserQuesId = table.Column<decimal>(type: "numeric(18, 0)", nullable: true),
                    IntwUserId = table.Column<int>(nullable: true),
                    IntwQusOptionsId = table.Column<int>(nullable: true),
                    IntwUserSessionId = table.Column<int>(nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntwUserAnswer", x => x.IntwUserAnswerId);
                    table.ForeignKey(
                        name: "FK_IntwUserAnswer_IntwQusOptions",
                        column: x => x.IntwQusOptionsId,
                        principalTable: "IntwQusOptions",
                        principalColumn: "IntwQusOptionsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntwUserAnswer_IntwUser",
                        column: x => x.IntwUserId,
                        principalTable: "IntwUser",
                        principalColumn: "IntwUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntwUserAnswer_IntwUserQues",
                        column: x => x.IntwUserQuesId,
                        principalTable: "IntwUserQues",
                        principalColumn: "IntwUserQuesId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntwUserAnswer_IntwUserSession",
                        column: x => x.IntwUserSessionId,
                        principalTable: "IntwUserSession",
                        principalColumn: "IntwUserSessionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertMessage_Uid",
                table: "AlertMessage",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_AlertMessageRead_AlertId",
                table: "AlertMessageRead",
                column: "AlertId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertMessageRead_Uid",
                table: "AlertMessageRead",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_Appraisal_Uid",
                table: "Appraisal",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_AppraisalExtras_AppraisalId",
                table: "AppraisalExtras",
                column: "AppraisalId");

            migrationBuilder.CreateIndex(
                name: "IX_AvailUser_UID",
                table: "AvailUser",
                column: "UID");

            migrationBuilder.CreateIndex(
                name: "IX_AvailUser_UserID",
                table: "AvailUser",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "unique_modelcode",
                table: "BucketModel",
                column: "ModelCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BucketModel",
                table: "BucketModel",
                column: "ModelName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CadidateExam_CandidateID",
                table: "CadidateExam",
                column: "CandidateID");

            migrationBuilder.CreateIndex(
                name: "IX_CadidateExam_ExamID",
                table: "CadidateExam",
                column: "ExamID");

            migrationBuilder.CreateIndex(
                name: "uc_Candidate",
                table: "Candidate",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateAnswer_CQuestionId",
                table: "CandidateAnswer",
                column: "CQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDocument_DepartmentId",
                table: "CompanyDocument",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Component_ComponentCategoryId",
                table: "Component",
                column: "ComponentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Component_CreatedByUid",
                table: "Component",
                column: "CreatedByUid");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentOpening_DepartmentId",
                table: "CurrentOpening",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "UQ__Departme__AC900526263B8EAF",
                table: "Department",
                column: "Deptcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Department",
                table: "Department",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DesignerManagement_AddedUID",
                table: "DesignerManagement",
                column: "AddedUID");

            migrationBuilder.CreateIndex(
                name: "IX_DesignerManagement_AssignUID",
                table: "DesignerManagement",
                column: "AssignUID");

            migrationBuilder.CreateIndex(
                name: "IX_DesignerManagement_ProjectId",
                table: "DesignerManagement",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDeviceHistory_DeviceDeviceInfoId",
                table: "DeviceDeviceHistory",
                column: "DeviceDeviceInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDeviceHistory_Uid",
                table: "DeviceDeviceHistory",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDeviceInfo_DeviceCategoryId",
                table: "DeviceDeviceInfo",
                column: "DeviceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDeviceInfo_PmId",
                table: "DeviceDeviceInfo",
                column: "PmId");

            migrationBuilder.CreateIndex(
                name: "IX_ElanceAssignedJob_ElanceJobId",
                table: "ElanceAssignedJob",
                column: "ElanceJobId");

            migrationBuilder.CreateIndex(
                name: "IX_ElanceAssignedJob_UserId",
                table: "ElanceAssignedJob",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLateActivity_Uid",
                table: "EmpLateActivity",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAnswers_AppraisalId",
                table: "EmployeeAnswers",
                column: "AppraisalId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAnswers_Qid",
                table: "EmployeeAnswers",
                column: "Qid");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAppraise_EmployeeId",
                table: "EmployeeAppraise",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAppraise_UserId",
                table: "EmployeeAppraise",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeComplaint_EmployeeId",
                table: "EmployeeComplaint",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeComplaint_UserId",
                table: "EmployeeComplaint",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeFeedback_Uid",
                table: "EmployeeFeedback",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeFeedbackRankStatus_EmployeeFeedbackId",
                table: "EmployeeFeedbackRankStatus",
                column: "EmployeeFeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeFeedbackReasonMapping_EmployeeFeedbackReasonId",
                table: "EmployeeFeedbackReasonMapping",
                column: "EmployeeFeedbackReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMedicalData_UserId",
                table: "EmployeeMedicalData",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProject_ProjectId",
                table: "EmployeeProject",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProject_Role",
                table: "EmployeeProject",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRelativeMedicalData_EmployeeMedicalId",
                table: "EmployeeRelativeMedicalData",
                column: "EmployeeMedicalId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateDocument_LeadId",
                table: "EstimateDocument",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateDocument_Uid_UploadedBy",
                table: "EstimateDocument",
                column: "Uid_UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestion_QuestionLevel",
                table: "ExamQuestion",
                column: "QuestionLevel");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestion_QuestionType",
                table: "ExamQuestion",
                column: "QuestionType");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestion_TechnologyID",
                table: "ExamQuestion",
                column: "TechnologyID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestionAnswerDetail_QuestID",
                table: "ExamQuestionAnswerDetail",
                column: "QuestID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestionDetail_QuestionID",
                table: "ExamQuestionDetail",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "uc_ExamQuestionDetail",
                table: "ExamQuestionDetail",
                columns: new[] { "ExamID", "QuestionID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forecasting_AddedPersonUId",
                table: "Forecasting",
                column: "AddedPersonUId");

            migrationBuilder.CreateIndex(
                name: "IX_Forecasting_ClientId",
                table: "Forecasting",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Forecasting_LeadId",
                table: "Forecasting",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Forecasting_ProjectId",
                table: "Forecasting",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumFeedback_ForumId",
                table: "ForumFeedback",
                column: "ForumId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumFeedback_UserId",
                table: "ForumFeedback",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Forums_UserId",
                table: "Forums",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FullLeave_AppraisalId",
                table: "FullLeave",
                column: "AppraisalId");

            migrationBuilder.CreateIndex(
                name: "IX_FullLeave_UserId",
                table: "FullLeave",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HalfLeave_AppraisalId",
                table: "HalfLeave",
                column: "AppraisalId");

            migrationBuilder.CreateIndex(
                name: "IX_HalfLeave_UserId",
                table: "HalfLeave",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IntwQues_IntwQuestypeId",
                table: "IntwQues",
                column: "IntwQuestypeId");

            migrationBuilder.CreateIndex(
                name: "IX_IntwQues_IntwTechnologyId",
                table: "IntwQues",
                column: "IntwTechnologyId");

            migrationBuilder.CreateIndex(
                name: "IX_IntwQuesExp_IntwExperienceId",
                table: "IntwQuesExp",
                column: "IntwExperienceId");

            migrationBuilder.CreateIndex(
                name: "IX_IntwQuesExp_IntwQuesId",
                table: "IntwQuesExp",
                column: "IntwQuesId");

            migrationBuilder.CreateIndex(
                name: "IX_IntwQusOptions_IntwQuesId",
                table: "IntwQusOptions",
                column: "IntwQuesId");

            migrationBuilder.CreateIndex(
                name: "UQ__IntwUser__7ED91AEE2D67AF2B",
                table: "IntwUser",
                column: "EmailID",
                unique: true,
                filter: "[EmailID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_IntwUser_IntwTechnologyId",
                table: "IntwUser",
                column: "IntwTechnologyId");

            migrationBuilder.CreateIndex(
                name: "IX_IntwUserAnswer_IntwQusOptionsId",
                table: "IntwUserAnswer",
                column: "IntwQusOptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_IntwUserAnswer_IntwUserId",
                table: "IntwUserAnswer",
                column: "IntwUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IntwUserAnswer_IntwUserQuesId",
                table: "IntwUserAnswer",
                column: "IntwUserQuesId");

            migrationBuilder.CreateIndex(
                name: "IX_IntwUserAnswer_IntwUserSessionId",
                table: "IntwUserAnswer",
                column: "IntwUserSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_IntwUserQues_IntwQuesid",
                table: "IntwUserQues",
                column: "IntwQuesid");

            migrationBuilder.CreateIndex(
                name: "IX_IntwUserQues_IntwUserSessionid",
                table: "IntwUserQues",
                column: "IntwUserSessionid");

            migrationBuilder.CreateIndex(
                name: "IX_IntwUserQues_IntwUserid",
                table: "IntwUserQues",
                column: "IntwUserid");

            migrationBuilder.CreateIndex(
                name: "IX_IntwUserSession_IntwQuesExpId",
                table: "IntwUserSession",
                column: "IntwQuesExpId");

            migrationBuilder.CreateIndex(
                name: "IX_IntwUserSession_IntwUserId",
                table: "IntwUserSession",
                column: "IntwUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobReference_CurrentOpeningId",
                table: "JobReference",
                column: "CurrentOpeningId");

            migrationBuilder.CreateIndex(
                name: "IX_JobReference_ReferBy_UserLoginId",
                table: "JobReference",
                column: "ReferBy_UserLoginId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBase_UserId",
                table: "KnowledgeBase",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDepartment_CodeId",
                table: "KnowledgeDepartment",
                column: "CodeId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDepartment_DeptID",
                table: "KnowledgeDepartment",
                column: "DeptID");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeTech_CodeId",
                table: "KnowledgeTech",
                column: "CodeId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeTech_TechId",
                table: "KnowledgeTech",
                column: "TechId");

            migrationBuilder.CreateIndex(
                name: "IX_LateHour_ModifiedByUid",
                table: "LateHour",
                column: "ModifiedByUid");

            migrationBuilder.CreateIndex(
                name: "IX_LateHour_Uid",
                table: "LateHour",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "UQ_Email",
                table: "LeadClient",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__LeadStat__05E7698A52442E1F",
                table: "LeadStatus",
                column: "StatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadTechnician_TechnicianId",
                table: "LeadTechnician",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "UQ_LeadId_TechnicianId",
                table: "LeadTechnician",
                columns: new[] { "LeadId", "TechnicianId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadTechnicianArchive_LeadId",
                table: "LeadTechnicianArchive",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadTechnicianArchive_TechnicianId",
                table: "LeadTechnicianArchive",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadTransaction_AddedBy",
                table: "LeadTransaction",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LeadTransaction_LeadId",
                table: "LeadTransaction",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadTransaction_StatusId",
                table: "LeadTransaction",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadTransactionArchive_AddedBy",
                table: "LeadTransactionArchive",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LeadTransactionArchive_LeadId",
                table: "LeadTransactionArchive",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadTransactionArchive_StatusId",
                table: "LeadTransactionArchive",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveActivity_AdjustID",
                table: "LeaveActivity",
                column: "AdjustID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveActivity_LeaveType",
                table: "LeaveActivity",
                column: "LeaveType");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveActivity_Status",
                table: "LeaveActivity",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveActivity_Uid",
                table: "LeaveActivity",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveActivity_WorkAlterID",
                table: "LeaveActivity",
                column: "WorkAlterID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveActivityAdjust_Adjustid",
                table: "LeaveActivityAdjust",
                column: "Adjustid");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveActivityAdjust_LeaveId",
                table: "LeaveActivityAdjust",
                column: "LeaveId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAdjust_Uid",
                table: "LeaveAdjust",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_MenuAccess_MenuId",
                table: "MenuAccess",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuAccess_RoleId",
                table: "MenuAccess",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_PFReviewResult_PFReviewQuestionId",
                table: "PFReviewResult",
                column: "PFReviewQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_PFReviewResult_PFReviewSubmittedId",
                table: "PFReviewResult",
                column: "PFReviewSubmittedId");

            migrationBuilder.CreateIndex(
                name: "IX_PFReviewSubmitted_ReviewBy_Uid",
                table: "PFReviewSubmitted",
                column: "ReviewBy_Uid");

            migrationBuilder.CreateIndex(
                name: "IX_PFReviewSubmitted_ReviewOn_Uid",
                table: "PFReviewSubmitted",
                column: "ReviewOn_Uid");

            migrationBuilder.CreateIndex(
                name: "IX_PFReviewSubmitted_ReviewQuarter",
                table: "PFReviewSubmitted",
                column: "ReviewQuarter");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolio_DeveloperId",
                table: "Portfolio",
                column: "DeveloperId");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolio_Domain_DomainId",
                table: "Portfolio_Domain",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolio_Domain_PortfolioID",
                table: "Portfolio_Domain",
                column: "PortfolioID");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolio_Tech_PortfolioID",
                table: "Portfolio_Tech",
                column: "PortfolioID");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolio_Tech_TechId",
                table: "Portfolio_Tech",
                column: "TechId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_AbroadPMUid",
                table: "Project",
                column: "AbroadPMUid");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ClientId",
                table: "Project",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Model",
                table: "Project",
                column: "Model");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Department_DeptID",
                table: "Project_Department",
                column: "DeptID");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Department_ProjectID",
                table: "Project_Department",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Tech_ProjectID",
                table: "Project_Tech",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Tech_TechId",
                table: "Project_Tech",
                column: "TechId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectClosure_AddedBy",
                table: "ProjectClosure",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectClosure_ProjectID",
                table: "ProjectClosure",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectClosure_Uid_BA",
                table: "ProjectClosure",
                column: "Uid_BA");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectClosure_Uid_Dev",
                table: "ProjectClosure",
                column: "Uid_Dev");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectClosure_Uid_TL",
                table: "ProjectClosure",
                column: "Uid_TL");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectClosureDetail_AddedByUid",
                table: "ProjectClosureDetail",
                column: "AddedByUid");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectClosureDetail_ProjectClosureId",
                table: "ProjectClosureDetail",
                column: "ProjectClosureId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeveloper_ProjectId",
                table: "ProjectDeveloper",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeveloper_Uid",
                table: "ProjectDeveloper",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeveloper_VD_id",
                table: "ProjectDeveloper",
                column: "VD_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeveloper_WorkStatus",
                table: "ProjectDeveloper",
                column: "WorkStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeveloperAddon_Uid",
                table: "ProjectDeveloperAddon",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeveloperAddon_WorkRole",
                table: "ProjectDeveloperAddon",
                column: "WorkRole");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeveloperAddon_WorkStatus",
                table: "ProjectDeveloperAddon",
                column: "WorkStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvoice_CurrencyID",
                table: "ProjectInvoice",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvoice_ProjectId",
                table: "ProjectInvoice",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvoice_Uid_BA",
                table: "ProjectInvoice",
                column: "Uid_BA");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvoice_Uid_TL",
                table: "ProjectInvoice",
                column: "Uid_TL");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvoiceComment_ProjectInvoiceId",
                table: "ProjectInvoiceComment",
                column: "ProjectInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLead_AbroadPMID",
                table: "ProjectLead",
                column: "AbroadPMID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLead_CommunicatorId",
                table: "ProjectLead",
                column: "CommunicatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLead_LeadClientId",
                table: "ProjectLead",
                column: "LeadClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLead_LeadType",
                table: "ProjectLead",
                column: "LeadType");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLead_OwnerId",
                table: "ProjectLead",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLead_Status",
                table: "ProjectLead",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "UQ__ProjectL__508FDAD217E28260",
                table: "ProjectLead",
                column: "TitleCheckSum",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeadArchive_AbroadPMID",
                table: "ProjectLeadArchive",
                column: "AbroadPMID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeadArchive_CommunicatorId",
                table: "ProjectLeadArchive",
                column: "CommunicatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeadArchive_LeadClientId",
                table: "ProjectLeadArchive",
                column: "LeadClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeadArchive_LeadType",
                table: "ProjectLeadArchive",
                column: "LeadType");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeadArchive_OwnerId",
                table: "ProjectLeadArchive",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeadArchive_Status",
                table: "ProjectLeadArchive",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeadTech_LeadId",
                table: "ProjectLeadTech",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeadTech_TechId",
                table: "ProjectLeadTech",
                column: "TechId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeadTechArchive_LeadId",
                table: "ProjectLeadTechArchive",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeadTechArchive_TechId",
                table: "ProjectLeadTechArchive",
                column: "TechId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPM_UserId",
                table: "ProjectPM",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadMessage_ForumId",
                table: "ReadMessage",
                column: "ForumId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadMessage_Uid",
                table: "ReadMessage",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_ReportBug_UserId",
                table: "ReportBug",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "unq_PresentDt",
                table: "SaturdayManagement",
                columns: new[] { "UID", "SaturdayDt" },
                unique: true,
                filter: "[UID] IS NOT NULL AND [SaturdayDt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SubProjects_ProjectId",
                table: "SubProjects",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_AddedUid",
                table: "Task",
                column: "AddedUid");

            migrationBuilder.CreateIndex(
                name: "IX_Task_TaskStatusID",
                table: "Task",
                column: "TaskStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignedTo_AssignUid",
                table: "TaskAssignedTo",
                column: "AssignUid");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignedTo_TaskID",
                table: "TaskAssignedTo",
                column: "TaskID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComment_AddedUid",
                table: "TaskComment",
                column: "AddedUid");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComment_TaskID",
                table: "TaskComment",
                column: "TaskID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComment_TaskStatusID",
                table: "TaskComment",
                column: "TaskStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Spec_TechId",
                table: "User_Spec",
                column: "TechId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Spec_Uid",
                table: "User_Spec",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_User_Tech_TechId",
                table: "User_Tech",
                column: "TechId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Tech_Uid",
                table: "User_Tech",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivity_ProjectId",
                table: "UserActivity",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivity_Uid",
                table: "UserActivity",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLog_ProjectId",
                table: "UserActivityLog",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLog_Uid",
                table: "UserActivityLog",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_UserLog_Uid",
                table: "UserLog",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_BloodGroupId",
                table: "UserLogin",
                column: "BloodGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_DeptId",
                table: "UserLogin",
                column: "DeptId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_RoleId",
                table: "UserLogin",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "uc_UserLogin",
                table: "UserLogin",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserTimeSheet_ProjectID",
                table: "UserTimeSheet",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_UserTimeSheet_ReviewedByUid",
                table: "UserTimeSheet",
                column: "ReviewedByUid");

            migrationBuilder.CreateIndex(
                name: "IX_UserTimeSheet_UID",
                table: "UserTimeSheet",
                column: "UID");

            migrationBuilder.CreateIndex(
                name: "IX_UserTimeSheet_VirtualDeveloper_id",
                table: "UserTimeSheet",
                column: "VirtualDeveloper_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertMessageRead");

            migrationBuilder.DropTable(
                name: "AppraisalExtras");

            migrationBuilder.DropTable(
                name: "AvailUser");

            migrationBuilder.DropTable(
                name: "CadidateExam");

            migrationBuilder.DropTable(
                name: "CandidateAnswer");

            migrationBuilder.DropTable(
                name: "Communication");

            migrationBuilder.DropTable(
                name: "CompanyDocument");

            migrationBuilder.DropTable(
                name: "Component");

            migrationBuilder.DropTable(
                name: "DailyThought");

            migrationBuilder.DropTable(
                name: "DesignerManagement");

            migrationBuilder.DropTable(
                name: "DeviceDeviceHistory");

            migrationBuilder.DropTable(
                name: "ElanceAssignedJob");

            migrationBuilder.DropTable(
                name: "ElanceCredential");

            migrationBuilder.DropTable(
                name: "EmpLateActivity");

            migrationBuilder.DropTable(
                name: "EmployeeActivity");

            migrationBuilder.DropTable(
                name: "EmployeeAnswers");

            migrationBuilder.DropTable(
                name: "EmployeeAppraise");

            migrationBuilder.DropTable(
                name: "EmployeeComplaint");

            migrationBuilder.DropTable(
                name: "EmployeeFeedbackRank");

            migrationBuilder.DropTable(
                name: "EmployeeFeedbackRankStatus");

            migrationBuilder.DropTable(
                name: "EmployeeFeedbackReasonMapping");

            migrationBuilder.DropTable(
                name: "EmployeeProject");

            migrationBuilder.DropTable(
                name: "EmployeeRelativeMedicalData");

            migrationBuilder.DropTable(
                name: "EstimateDocument");

            migrationBuilder.DropTable(
                name: "ExamQuestionAnswerDetail");

            migrationBuilder.DropTable(
                name: "ExamQuestionDetail");

            migrationBuilder.DropTable(
                name: "Forecasting");

            migrationBuilder.DropTable(
                name: "ForumFeedback");

            migrationBuilder.DropTable(
                name: "FullLeave");

            migrationBuilder.DropTable(
                name: "HalfLeave");

            migrationBuilder.DropTable(
                name: "IntwUserAnswer");

            migrationBuilder.DropTable(
                name: "JobReference");

            migrationBuilder.DropTable(
                name: "KnowledgeDepartment");

            migrationBuilder.DropTable(
                name: "KnowledgeTech");

            migrationBuilder.DropTable(
                name: "LateHour");

            migrationBuilder.DropTable(
                name: "Leadership");

            migrationBuilder.DropTable(
                name: "LeadTechnician");

            migrationBuilder.DropTable(
                name: "LeadTechnicianArchive");

            migrationBuilder.DropTable(
                name: "LeadTransaction");

            migrationBuilder.DropTable(
                name: "LeadTransactionArchive");

            migrationBuilder.DropTable(
                name: "LeaveActivityAdjust");

            migrationBuilder.DropTable(
                name: "Management");

            migrationBuilder.DropTable(
                name: "MeetingMinutes");

            migrationBuilder.DropTable(
                name: "MenuAccess");

            migrationBuilder.DropTable(
                name: "OfficialLeave");

            migrationBuilder.DropTable(
                name: "PersonalDevelopment");

            migrationBuilder.DropTable(
                name: "PFReviewResult");

            migrationBuilder.DropTable(
                name: "Portfolio_Domain");

            migrationBuilder.DropTable(
                name: "Portfolio_Tech");

            migrationBuilder.DropTable(
                name: "Preferences");

            migrationBuilder.DropTable(
                name: "Productivity");

            migrationBuilder.DropTable(
                name: "Project_Department");

            migrationBuilder.DropTable(
                name: "Project_Tech");

            migrationBuilder.DropTable(
                name: "ProjectClose");

            migrationBuilder.DropTable(
                name: "ProjectClosureDetail");

            migrationBuilder.DropTable(
                name: "ProjectDeveloper");

            migrationBuilder.DropTable(
                name: "ProjectDeveloperAddon");

            migrationBuilder.DropTable(
                name: "ProjectInvoiceComment");

            migrationBuilder.DropTable(
                name: "ProjectLeadTech");

            migrationBuilder.DropTable(
                name: "ProjectLeadTechArchive");

            migrationBuilder.DropTable(
                name: "ProjectPM");

            migrationBuilder.DropTable(
                name: "ReadMessage");

            migrationBuilder.DropTable(
                name: "Relationship");

            migrationBuilder.DropTable(
                name: "ReportBug");

            migrationBuilder.DropTable(
                name: "SaturdayManagement");

            migrationBuilder.DropTable(
                name: "SubProjects");

            migrationBuilder.DropTable(
                name: "TaskAssignedTo");

            migrationBuilder.DropTable(
                name: "TaskComment");

            migrationBuilder.DropTable(
                name: "User_Spec");

            migrationBuilder.DropTable(
                name: "User_Tech");

            migrationBuilder.DropTable(
                name: "UserActivity");

            migrationBuilder.DropTable(
                name: "UserActivityLog");

            migrationBuilder.DropTable(
                name: "UserLog");

            migrationBuilder.DropTable(
                name: "UserTimeSheet");

            migrationBuilder.DropTable(
                name: "AlertMessage");

            migrationBuilder.DropTable(
                name: "Candidate");

            migrationBuilder.DropTable(
                name: "ComponentCategory");

            migrationBuilder.DropTable(
                name: "DeviceDeviceInfo");

            migrationBuilder.DropTable(
                name: "ElanceJobDetails");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "EmployeeFeedback");

            migrationBuilder.DropTable(
                name: "EmployeeFeedbackReason");

            migrationBuilder.DropTable(
                name: "EmployeeMedicalData");

            migrationBuilder.DropTable(
                name: "Examination");

            migrationBuilder.DropTable(
                name: "ExamQuestion");

            migrationBuilder.DropTable(
                name: "Appraisal");

            migrationBuilder.DropTable(
                name: "IntwQusOptions");

            migrationBuilder.DropTable(
                name: "IntwUserQues");

            migrationBuilder.DropTable(
                name: "CurrentOpening");

            migrationBuilder.DropTable(
                name: "KnowledgeBase");

            migrationBuilder.DropTable(
                name: "LeaveActivity");

            migrationBuilder.DropTable(
                name: "FrontMenu");

            migrationBuilder.DropTable(
                name: "PFReviewQuestion");

            migrationBuilder.DropTable(
                name: "PFReviewSubmitted");

            migrationBuilder.DropTable(
                name: "DomainType");

            migrationBuilder.DropTable(
                name: "Portfolio");

            migrationBuilder.DropTable(
                name: "ProjectClosure");

            migrationBuilder.DropTable(
                name: "ProjectInvoice");

            migrationBuilder.DropTable(
                name: "ProjectLead");

            migrationBuilder.DropTable(
                name: "ProjectLeadArchive");

            migrationBuilder.DropTable(
                name: "Forums");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "VirtualDeveloper");

            migrationBuilder.DropTable(
                name: "DeviceCategory");

            migrationBuilder.DropTable(
                name: "Technology");

            migrationBuilder.DropTable(
                name: "IntwUserSession");

            migrationBuilder.DropTable(
                name: "LeaveAdjust");

            migrationBuilder.DropTable(
                name: "PFReviewQuarter");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "AbroadPM");

            migrationBuilder.DropTable(
                name: "LeadClient");

            migrationBuilder.DropTable(
                name: "TypeMaster");

            migrationBuilder.DropTable(
                name: "LeadStatus");

            migrationBuilder.DropTable(
                name: "TaskStatus");

            migrationBuilder.DropTable(
                name: "IntwQuesExp");

            migrationBuilder.DropTable(
                name: "IntwUser");

            migrationBuilder.DropTable(
                name: "UserLogin");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "BucketModel");

            migrationBuilder.DropTable(
                name: "IntwExperience");

            migrationBuilder.DropTable(
                name: "IntwQues");

            migrationBuilder.DropTable(
                name: "BloodGroup");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "IntwQuestype");

            migrationBuilder.DropTable(
                name: "IntwTechnology");
        }
    }
}
