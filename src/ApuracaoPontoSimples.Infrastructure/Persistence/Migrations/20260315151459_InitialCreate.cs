using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApuracaoPontoSimples.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Cnpj = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Pis = table.Column<string>(type: "text", nullable: true),
                    AdmissionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EmployerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Employers_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "Employers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DailyHours = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DailyLimit = table.Column<TimeSpan>(type: "interval", nullable: true),
                    SaturdayHours = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ToleranceEntry = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ToleranceExit = table.Column<TimeSpan>(type: "interval", nullable: true),
                    NightStart = table.Column<TimeSpan>(type: "interval", nullable: true),
                    NightEnd = table.Column<TimeSpan>(type: "interval", nullable: true),
                    WeeklyHours = table.Column<TimeSpan>(type: "interval", nullable: true),
                    SaturdayCountsAsBank = table.Column<bool>(type: "boolean", nullable: false),
                    UseDailyHoursAsY13 = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeCards_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DayEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Interval1_Start = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Interval1_End = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Interval2_Start = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Interval2_End = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Interval3_Start = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Interval3_End = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HolidayType = table.Column<int>(type: "integer", nullable: false),
                    IsSunday = table.Column<bool>(type: "boolean", nullable: false),
                    IsSaturday = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DayEntries_TimeCards_TimeCardId",
                        column: x => x.TimeCardId,
                        principalTable: "TimeCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeCardTotals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    BancoHorasConvTotal = table.Column<double>(type: "double precision", nullable: false),
                    BancoHorasTotal = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ApuracaoConvTotal = table.Column<double>(type: "double precision", nullable: false),
                    ApuracaoTotal = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HorasPositivasTotal = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HorasPositivasConvTotal = table.Column<double>(type: "double precision", nullable: false),
                    HorasNegativasTotal = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HorasNegativasConvTotal = table.Column<double>(type: "double precision", nullable: false),
                    HorasNoturnasTotal = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HorasNoturnasConvTotal = table.Column<double>(type: "double precision", nullable: false),
                    HorasExtrasTotal = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HorasExtrasConvTotal = table.Column<double>(type: "double precision", nullable: false),
                    HorasDsrFeriadoTotal = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HorasDsrFeriadoConvTotal = table.Column<double>(type: "double precision", nullable: false),
                    FechamentoSemanaPositivoTotal = table.Column<TimeSpan>(type: "interval", nullable: false),
                    FechamentoSemanaNegativoTotal = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeCardTotals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeCardTotals_TimeCards_TimeCardId",
                        column: x => x.TimeCardId,
                        principalTable: "TimeCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Absences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DayEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Hours = table.Column<TimeSpan>(type: "interval", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Absences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Absences_DayEntries_DayEntryId",
                        column: x => x.DayEntryId,
                        principalTable: "DayEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DayCalculation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DayEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Apuracao = table.Column<TimeSpan>(type: "interval", nullable: true),
                    BancoHoras = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HorasPositivas = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HorasNegativas = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HorasExtras = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HorasDsrFeriado = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HorasNoturnas = table.Column<TimeSpan>(type: "interval", nullable: true),
                    BancoHorasConv = table.Column<double>(type: "double precision", nullable: true),
                    ApuracaoConv = table.Column<double>(type: "double precision", nullable: true),
                    HorasPositivasConv = table.Column<double>(type: "double precision", nullable: true),
                    HorasNegativasConv = table.Column<double>(type: "double precision", nullable: true),
                    HorasExtrasConv = table.Column<double>(type: "double precision", nullable: true),
                    HorasDsrFeriadoConv = table.Column<double>(type: "double precision", nullable: true),
                    HorasNoturnasConv = table.Column<double>(type: "double precision", nullable: true),
                    AcumuladoSemana = table.Column<TimeSpan>(type: "interval", nullable: true),
                    FechamentoSemanaPositivo = table.Column<TimeSpan>(type: "interval", nullable: true),
                    FechamentoSemanaNegativo = table.Column<TimeSpan>(type: "interval", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayCalculation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DayCalculation_DayEntries_DayEntryId",
                        column: x => x.DayEntryId,
                        principalTable: "DayEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Absences_DayEntryId",
                table: "Absences",
                column: "DayEntryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DayCalculation_DayEntryId",
                table: "DayCalculation",
                column: "DayEntryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DayEntries_TimeCardId",
                table: "DayEntries",
                column: "TimeCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployerId",
                table: "Employees",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_EmployeeId",
                table: "Schedules",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeCards_EmployeeId",
                table: "TimeCards",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeCardTotals_TimeCardId",
                table: "TimeCardTotals",
                column: "TimeCardId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Absences");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "DayCalculation");

            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "TimeCardTotals");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "DayEntries");

            migrationBuilder.DropTable(
                name: "TimeCards");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Employers");
        }
    }
}
