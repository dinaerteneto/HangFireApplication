using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HangFireApplication.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreateTaskSchedulerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskSchedulers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExecutablePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Arguments = table.Column<string>(type: "nvarchar(max)", nullable: true), // Corrigido para nullable
                    CronExpression = table.Column<string>(type: "nvarchar(max)", nullable: false), // Nome corrigido
                    IntervalInMinutes = table.Column<int>(type: "int", nullable: false),
                    Enable = table.Column<bool>(type: "bit", nullable: false), // Adicionado ou removido conforme necessário
                    DependentTaskId = table.Column<Guid>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSchedulers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskSchedulers_TaskSchedulers_DependentTaskId",
                        column: x => x.DependentTaskId,
                        principalTable: "TaskSchedulers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_TaskSchedulers_DependentTaskId",
                table: "TaskSchedulers",
                column: "DependentTaskId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskSchedulers"
            );
        }
    }
}
