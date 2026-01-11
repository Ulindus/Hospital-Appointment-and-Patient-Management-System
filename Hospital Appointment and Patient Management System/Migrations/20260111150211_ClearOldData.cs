using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_Appointment_and_Patient_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class ClearOldData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Appointments");
            migrationBuilder.Sql("DELETE FROM DoctorSchedules");
            migrationBuilder.Sql("DELETE FROM Patients");
            migrationBuilder.Sql("DELETE FROM Doctors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
