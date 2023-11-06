using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrafficCourts.Workflow.Service.Migrations.VerifyEmailAddressStateMigrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VerifyEmailAddressState",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", maxLength: 64, nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TicketNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Token = table.Column<Guid>(type: "uuid", nullable: false),
                    Verified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DisputeId = table.Column<long>(type: "bigint", nullable: true),
                    IsUpdateEmailVerification = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerifyEmailAddressState", x => x.CorrelationId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerifyEmailAddressState");
        }
    }
}
